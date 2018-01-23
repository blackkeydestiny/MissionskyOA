using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using System.Collections.ObjectModel;
using MissionskyOA.Core.Common;

namespace MissionskyOA.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly INotificationService _notificationService = new NotificationService();
        /// <summary>
        /// 获取会议室信息
        /// </summary>
        /// <returns>获取会议室信息</returns>
        public List<MeetingRoomModel> GetMeetingRoomList()
        {
            using (var dbcontext= new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingRooms.Where(it => it.Status != 0);
                List<MeetingRoomModel> meetingRoomList = new List<MeetingRoomModel>();
                entity.ToList().ForEach(item =>
                {
                    meetingRoomList.Add(item.ToModel());
                });
                return meetingRoomList;
            }
        }

        public ListResult<MeetingRoomModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.MeetingRooms.AsEnumerable();

                //if (sort != null)
                //{

                //}

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<MeetingRoomModel> result = new ListResult<MeetingRoomModel>();
                result.Data = new List<MeetingRoomModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }


        /// <summary>
        /// 预定会议
        /// </summary>
        /// <returns>预定会议信息</returns>
        public MeetingCalendarModel AddMeeting(MeetingCalendarModel model,bool isAddMeetingByWeek)
        {
            var meetingCalendarEntity = model.ToEntity();
            //会议室占用
            meetingCalendarEntity.Status = 1;
            using (var dbcontext = new MissionskyOAEntities())
            {
                dbcontext.MeetingCalendars.Add(meetingCalendarEntity);
                model.MeetingParticipants.ToList().ForEach(item =>
                {
                    if (model.MeetingRoomId != null && model.MeetingRoomId != 0)
                    {

                        //Get meetingRoom
                        var meetingRoom = dbcontext.MeetingRooms.FirstOrDefault(it => it.Id == model.MeetingRoomId);
                        if (meetingRoom != null)
                        {
                            model.MeetingRoom = meetingRoom.ToModel();
                        }
                        else
                        {
                            throw new Exception("无效的会议室");
                        }

                    }
                    var userEntity = dbcontext.Users.Where(it => it.Id == item.UserId).FirstOrDefault();
                    var notificationModel = new NotificationModel()
                    {
                        Target = userEntity.Email,
                        CreatedUserId = model.ApplyUserId,
                        //MessageType = NotificationType.PushMessage,
                        MessageType = NotificationType.Email,
                        BusinessType = BusinessType.BookMeetingRoomSuccessAnnounce,
                        Title = "Missionsky OA Notification",
                        MessagePrams = "test",
                        Scope = NotificationScope.User,
                        CreatedTime = DateTime.Now,
                        TargetUserIds = new List<int> { item.UserId }
                    };
                    DateTime startDate = Convert.ToDateTime(model.StartDate.ToShortDateString() + " " + model.StartTime.ToString());
                    if (!isAddMeetingByWeek)
                    {
                        notificationModel.MessageContent = string.Format("请参加，于{0}召开由{1}主持的{2}会议。", startDate, model.Host, model.Title);
                        this._notificationService.Add(notificationModel, Global.IsProduction); //消息推送
                    }
                });
                dbcontext.SaveChanges();
            }

            return meetingCalendarEntity.ToModel();
        }


        /// <summary>
        /// 更新会议室信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool UpdateMeetingRoom(MeetingRoomModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingRooms.FirstOrDefault(it => it.Id == model.Id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("找不到会议室");
                }
                entity.MeetingRoomName = model.MeetingRoomName;
                entity.Equipment = model.Equipment;
                entity.Capacity = model.Capacity;
                entity.Status = model.Status;
                entity.Remark = model.Remark;
                dbcontext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 添加会议室信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool AddMeetingRoom(MeetingRoomModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                dbcontext.MeetingRooms.Add(model.ToEntity());
                dbcontext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 删除会议室信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool Remove(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingRooms.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("找不到会议室");
                }
                if(entity.Status==0)
                {
                    entity.Status = 1;
                }
                else if(entity.Status==1)
                {
                    entity.Status = 0;
                }
                dbcontext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 查询具体会议信息
        /// </summary>
        /// <returns>会议信息</returns>
        public MeetingCalendarModel GetMeetingDetailsById(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingCalendars.Where(it => it.Id == id).FirstOrDefault();
                if (entity == null)
                {
                    throw new Exception("无效的会议Id");
                }
                return entity.ToModel();
            }
        }

        /// <summary>
        /// 查询具体会议信息
        /// </summary>
        /// <returns>会议信息</returns>
        public MeetingCalendarModel GetMeetingDetailsById(MissionskyOAEntities dbcontext, int id)
        {
            var entity = dbcontext.MeetingCalendars.Where(it => it.Id == id).FirstOrDefault();
            if (entity == null)
            {
                throw new Exception("无效的会议Id");
            }
            return entity.ToModel();
        }

        /// <summary>
        /// 查询具体会议室信息
        /// </summary>
        /// <returns>查询具体会议室信息</returns>
        public MeetingRoomModel GetMeetingRoomById(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingRooms.Where(it => it.Id == id).FirstOrDefault();
                if (entity == null)
                {
                    throw new Exception("无效的Id");
                }
                return entity.ToModel();
            }
        }

        /// <summary>
        /// 会议室是否可用
        /// </summary>
        /// <returns>是否可用</returns>
        public bool isMeetingRoomAvailiable(int meetingRoomID, DateTime startDate, DateTime endDate)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                //开始时间是否在已存在会议的时段内
                var entityStarTimeExist = dbcontext.MeetingCalendars.Where(it => it.MeetingRoomId == meetingRoomID && it.StartDate == startDate.Date && it.EndDate == endDate.Date && startDate.TimeOfDay >= it.StartTime && startDate.TimeOfDay < it.EndTime&&it.Status==1).FirstOrDefault();
                //结束时间是否在已存在会议的时段内
                var entityEndTimeExist = dbcontext.MeetingCalendars.Where(it => it.MeetingRoomId == meetingRoomID && it.StartDate == startDate.Date && it.EndDate == endDate.Date && endDate.TimeOfDay > it.StartTime && endDate.TimeOfDay <= it.EndTime&&it.Status==1).FirstOrDefault();
                if ((entityStarTimeExist != null) || (entityEndTimeExist != null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 根据日期时间返回会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        public List<MeetingDateGroupModel> GetMeetingListByDateTime(MeetingSearchModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                //查询
                var entity = dbcontext.MeetingCalendars.Where(it => it.MeetingRoomId == model.MeetingRoomId && it.StartDate >= model.StartDate.Date && it.StartDate <= model.EndDate.Date&&it.Status==1).GroupBy(x => x.StartDate).AsEnumerable().Select(group => 
                new 
                {
                    startdate = group.Key,
                    Meetings=group.Where(it=>it.Id!=null).OrderBy(c=>c.StartTime)
                });

                if(entity==null)
                {
                    throw new Exception("此日期段内无会议信息");
                }
                //根据日期group返回会议记录
                List<MeetingDateGroupModel> meetingList = new List<MeetingDateGroupModel>();
                entity.ToList().ForEach(item =>
                {
                    MeetingDateGroupModel temp = new MeetingDateGroupModel() 
                    { 
                          meetings=new List<MeetingCalendarModel>()
                    };
                    temp.startDate = item.startdate;
                    item.Meetings.ToList().ForEach(i =>
                    {
                        temp.meetings.Add(i.ToModel());
                    });
                    meetingList.Add(temp);
                });
                return meetingList;

            }
        }

        /// <summary>
        /// 根据会议室ID,查询会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        public List<MeetingCalendarModel> GetMeetingListByMeetingRoomId(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                //查询
                var entity = dbcontext.MeetingCalendars.Where(it => it.MeetingRoomId == id);

                List<MeetingCalendarModel> meetingList = new List<MeetingCalendarModel>();
                entity.ToList().ForEach(item =>
                {
                    meetingList.Add(item.ToModel());
                });
                return meetingList;
            }
        }

        /// <summary>
        /// 根据用户ID,日期时间返回会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        public List<MeetingCalendarModel> GetUserReserveMeetingHistory(MeetingSearchModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                //查询
                var entity = dbcontext.MeetingCalendars.Where(it=>it.ApplyUserId==model.UserId&&it.StartDate >= model.StartDate.Date && it.StartDate <= model.EndDate.Date).OrderBy(item=>item.StartDate).ThenBy(a => a.StartTime);                
                //根据日期group返回会议记录
                List<MeetingCalendarModel> meetingList = new List<MeetingCalendarModel>();
                entity.ToList().ForEach(item =>
                {
                    meetingList.Add(item.ToModel());
                });
                return meetingList;
            }
        }

        public bool UpdateReserveMeetingHistory(int id, MeetingCalendarModel model,int userID)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingCalendars.Where(it => it.Id == id).FirstOrDefault();
                if(entity==null)
                {
                    throw new Exception("无效的Id");
                }
                if(userID!=entity.ApplyUserId)
                {
                    throw new Exception("当前用户没有修改权限，只有申请用户才能修改");
                }
                DateTime oldStartDate = Convert.ToDateTime(entity.StartDate.ToShortDateString() + " " + entity.StartTime.ToString());
                DateTime oldEndDate = Convert.ToDateTime(entity.EndDate.ToShortDateString() + " " + entity.EndTime.ToString());
                //会议开始时间必须比现在时间晚
                if (oldEndDate <= DateTime.Now)
                {
                    throw new Exception("会议已经结束不能修改");
                }
                Collection<MeetingParticipant> result = new Collection<MeetingParticipant>();

                if (model.MeetingParticipants != null && model.MeetingParticipants.Count > 0)
                {
                    //移除老的参与者
                    var meetingParticipant = dbcontext.MeetingParticipants.Where(it => it.MeetingCalendarId == id);
                    meetingParticipant.ToList().ForEach(item =>
                    {
                        dbcontext.MeetingParticipants.Remove(item);
                    });

                    foreach (MeetingParticipantModel item in model.MeetingParticipants)
                    {
                        result.Add(item.ToEntity());
                    }
                    entity.MeetingParticipants = result;
                }
                if(model.StartDate.Date!=entity.StartDate)
                {
                    entity.StartDate = model.StartDate;
                }
                if (model.EndDate.Date != entity.EndDate)
                {
                    entity.EndDate = model.EndDate;
                }
                if (model.StartTime != entity.StartTime)
                {
                    entity.StartTime = model.StartTime;
                }
                if (model.EndTime != entity.EndTime)
                {
                    entity.EndTime = model.EndTime;
                }
                if(model.Title!=null&&!(model.Title.Equals(entity.Title)))
                {
                    entity.Title = model.Title;
                }
                if (model.MeetingContext != null && !(model.MeetingContext.Equals(entity.MeetingContext)))
                {
                    entity.MeetingContext = model.MeetingContext;
                }
                if (model.Host != null && !(model.Host.Equals(entity.Host)))
                {
                    entity.Host = model.Host;
                }
                dbcontext.SaveChanges();
                return true;
            }
        }


        /// <summary>
        /// 更新会议纪要
        /// </summary>
        /// <returns>更新会议纪要</returns>
        public bool UpdateMeetingSummary(MeetingCalendarModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingCalendars.Where(it => it.Id == model.Id).FirstOrDefault();
                if (entity == null)
                {
                    throw new Exception("无效的Id");
                }

                if (model.MeetingSummary != null && !(model.MeetingSummary.Equals(entity.MeetingSummary)))
                {
                    entity.MeetingSummary = model.MeetingSummary;
                }
                dbcontext.SaveChanges();
                return true;
            }

        }

        /// <summary>
        /// 取消会议预定
        /// </summary>
        /// <returns>取消成功：true 失败：false</returns>
        public bool cancelMeeting(int id,int userID)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.MeetingCalendars.Where(it => it.Id == id).FirstOrDefault();
                if (entity == null)
                {
                    throw new Exception("无效的Id");
                }
                if (userID != entity.ApplyUserId)
                {
                    throw new Exception("当前用户没有修改权限，只有申请用户才能修改");
                }

                DateTime startDateInDB = Convert.ToDateTime(entity.StartDate.ToShortDateString() + " " + entity.StartTime.ToString());
                DateTime endDateInDB = Convert.ToDateTime(entity.EndDate.ToShortDateString() + " " + entity.EndTime.ToString());
                if(startDateInDB>=DateTime.Now)
                {
                    entity.MeetingParticipants.ToList().ForEach(item =>
                    {
                        var model = new NotificationModel()
                        {
                            Target = item.User.Email,
                            CreatedUserId = userID,
                            //MessageType = NotificationType.PushMessage,
                            MessageType = NotificationType.Email,
                            BusinessType = BusinessType.CacnelMeetingRoomSuccessAnnounce,
                            Title = "Missionsky OA Notification",
                            MessagePrams = "test",
                            Scope = NotificationScope.User,
                            CreatedTime = DateTime.Now,
                            TargetUserIds = new List<int> { item.User.Id }
                        };

                        model.MessageContent = string.Format("于{0}召开由{1}主持的{2}会议已经取消。",startDateInDB,entity.Host,entity.Title);

                        dbcontext.MeetingParticipants.Remove(item);
                        this._notificationService.Add(model, Global.IsProduction); //消息推送
                    });
                    dbcontext.MeetingCalendars.Remove(entity);
                }
                else if(startDateInDB<DateTime.Now)
                {
                    //entity.Status = 0;
                    //entity.MeetingParticipants.ToList().ForEach(item =>
                    //{
                    //    var model = new NotificationModel()
                    //    {
                    //        Target = item.User.Email,
                    //        CreatedUserId = userID,
                    //        MessageType = NotificationType.PushMessage,
                    //        BusinessType = BusinessType.CacnelMeetingRoomSuccessAnnounce,
                    //        Title = "Missionsky OA Notification",
                    //        MessagePrams = "test",
                    //        Scope = NotificationScope.User,
                    //        CreatedTime = DateTime.Now,
                    //        TargetUserIds = new List<int> { item.User.Id }
                    //    };
                    //    model.MessageContent = string.Format("于{0}召开的{2}会议已经取消。", startDateInDB, String.IsNullOrEmpty(entity.Host) ? "" :("由" + entity.Host + "主持的"), entity.Title);
                    //    this._notificationService.Add(model, Global.IsProduction); //消息推送
                    //    dbcontext.MeetingParticipants.Remove(item);
                    //});
                    throw new Exception("会议已经开始，不能取消");

                }
                dbcontext.SaveChanges();
                return true;
            }
        }

    }
}
