using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Email;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Task
{
    /// <summary>
    /// 通知会议开始结束
    /// </summary>
    public class NotifyMeeting : TaskBase, ITaskRunnable
    {
        /// <summary>
        /// 今日已经会议通知(今日不再重复通知)
        /// </summary>
        private static readonly IList<MeetingNotificationModel> NotificationHistory = new List<MeetingNotificationModel>();

        /// <summary>
        /// 执行定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <param name="executeDate">执行时间</param>
        public void Execute(ScheduledTaskModel task, DateTime executeDate)
        {
            if (task == null)
            {
                Log.Error("无效的定时任务。");
                throw new InvalidOperationException("无效的定时任务。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var sql = @"SELECT * FROM MeetingCalendar
                            WHERE DATEDIFF(DAY, CAST(StartDate AS NVARCHAR), GETDATE()) = 0 OR DATEDIFF(DAY, CAST(EndDate AS NVARCHAR), GETDATE()) = 0
                            ORDER BY StartDate DESC, StartTime DESC;";

                //当天开始或结束的会议
                var entities = dbContext.MeetingCalendars.SqlQuery(sql).ToList();
                
                //定时任务详情
                var content = string.Empty;

                //当天开始或结束的会议，发送消息通知
                entities.ForEach(meeting =>
                {
                    if (CanNotify(meeting.ToModel(), MeetingNotificationType.StartNotification)) //会议开始通知
                    {
                        Notify(dbContext, meeting.ToModel(), MeetingNotificationType.StartNotification);
                        content = string.Format("{0},{1}", content, meeting.Title);
                    }

                    if (CanNotify(meeting.ToModel(), MeetingNotificationType.EndNotification)) //会议结束通知
                    {
                        Notify(dbContext, meeting.ToModel(), MeetingNotificationType.EndNotification);
                        content = string.Format("{0},{1}", content, meeting.Title);
                    }
                });

                //更新定时间任务
                content = string.IsNullOrEmpty(content) ? content : content.Substring(1);
                ScheduledTaskService.UpdateTask(dbContext, task.Id, executeDate, content);

                //更新数据库
                dbContext.SaveChanges();

                //移出过期的通知记录
                RemoveExpiredHistory();

                //Log.Info(string.Format("定时任务执行成功: {0}。", task.Name));
            }
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="meeting">会议</param>
        /// <param name="type">通知类型</param>
        private void Notify(MissionskyOAEntities dbContext, MeetingCalendarModel meeting, MeetingNotificationType type)
        {
            if (dbContext == null || meeting == null || type == MeetingNotificationType.None)
            {
                Log.Error(string.Format("会议通知失败, 通知类型: {0}。", type));
                throw new InvalidOperationException(string.Format("会议通知失败, 通知类型: {0}。", type));
            }

            //参会者
            IList<string> participants = new List<string>();

            dbContext.MeetingParticipants.Where(it => it.MeetingCalendarId == meeting.Id).ToList().ForEach(it =>
            {
                participants.Add(it.User.Email); //参会者

                var model = new NotificationModel()
                {
                    Target = it.User.Email,
                    CreatedUserId = 0,
                    //MessageType = NotificationType.PushMessage,
                    MessageType = NotificationType.Email,
                    Title = "会议开始结束通知",
                    MessagePrams = "test",
                    Scope = NotificationScope.User,
                    CreatedTime = DateTime.Now,
                    TargetUserIds = new List<int> {it.User.Id}
                };

                if (type == MeetingNotificationType.StartNotification)
                {
                    var startDate =
                        Convert.ToDateTime(meeting.StartDate.ToShortDateString() + " " + meeting.StartTime.ToString());

                    model.BusinessType = BusinessType.NotifyMeetingStart;
                    model.MessageContent = string.Format("由{0}主持的会议{1}将于{2}开始。", meeting.Host, meeting.Title, startDate);
                }
                else
                {
                    var endDate =
                        Convert.ToDateTime(meeting.EndDate.ToShortDateString() + " " + meeting.EndTime.ToString());

                    model.BusinessType = BusinessType.NotifyMeetingEnd;
                    model.MessageContent = string.Format("由{0}主持的会议{1}将于{2}结束。", meeting.Host, meeting.Title, endDate);
                }

                NotificationService.Add(model, Global.IsProduction); //消息推送
            });

            //添加通知历史
            NotificationHistory.Add(new MeetingNotificationModel()
            {
                Meeting = meeting.Id,
                NotifiedTime = DateTime.Now,
                Type = type
            });

            #region 发送邮件
            //会议纪要连接
            string meetingSummaryURL = string.Format(Global.MeetingSummaryURL, meeting.Id);

            //邮件内容
            string title = (type == MeetingNotificationType.StartNotification ? "[OA]会议开始通知" : "[OA]会议结束通知");
            string body = string.Format("会议主题：{0}\r\n会议议程：{1}\r\n会议纪要：{2}", meeting.Title, meeting.MeetingContext,
                meetingSummaryURL);

            EmailClient.Send(participants, null, title, body); //发送邮件
            #endregion
        }

        /// <summary>
        /// 提前5分钟提醒通知会议开始或会议结束
        /// </summary>
        /// <param name="meeting">会议</param>
        /// <param name="type">通知类型</param>
        /// <returns>true or false</returns>
        private bool CanNotify(MeetingCalendarModel meeting, MeetingNotificationType type)
        {
            if (meeting == null || type == MeetingNotificationType.None)
            {
                Log.Error(string.Format("会议通知失败, 通知类型: {0}。", type));
                throw new InvalidOperationException(string.Format("会议通知失败, 通知类型: {0}。", type));
            }

            //已经通知过，不再通知
            if (IsNotified(meeting.Id, type))
            {
                return false;
            }

            DateTime meetingDate;
            if (type == MeetingNotificationType.StartNotification) //开始通知
            {
                meetingDate =
                    Convert.ToDateTime(meeting.StartDate.ToShortDateString() + " " + meeting.StartTime.ToString());
            }
            else if (type == MeetingNotificationType.EndNotification) //结束通知
            {
                meetingDate =
                    Convert.ToDateTime(meeting.EndDate.ToShortDateString() + " " + meeting.EndTime.ToString());
            }
            else
            {
                return false;
            }

            var spanMeeting = new TimeSpan(meetingDate.Ticks);
            var spanNow = new TimeSpan(DateTime.Now.Ticks);
            var minutes = spanMeeting.Subtract(spanNow).TotalMinutes;

            //提前5分钟提醒
            return (minutes > 0 && minutes <= 5);
        }

        /// <summary>
        /// 是否已经发出会议通知
        /// </summary>
        /// <param name="meetingId">会议Id</param>
        /// <param name="type">会议通知类型</param>
        /// <returns>true or false</returns>
        private bool IsNotified(int meetingId, MeetingNotificationType type)
        {
            if (NotificationHistory == null)
            {
                Log.Error("会议通知失败。");
                throw new InvalidOperationException("会议通知失败。");
            }

            //已发送的通知
            var notification = NotificationHistory.FirstOrDefault(it => it.Meeting == meetingId && it.Type == type);

            //未通知过
            if (notification == null)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// 移出过期的通知记录
        /// </summary>
        private void RemoveExpiredHistory()
        {
            if (NotificationHistory == null)
            {
                Log.Error("会议通知失败。");
                throw new InvalidOperationException("会议通知失败。");
            }

            //过期通知
            var expiredHistory = NotificationHistory.Where(it => DateTime.Compare(it.NotifiedTime.Date, DateTime.Now.Date) < 0).ToList();

            //移出过期通知
            if (expiredHistory.Count > 0)
            {
                expiredHistory.ForEach(it => NotificationHistory.Remove(it));
            }
        }
    }
}
