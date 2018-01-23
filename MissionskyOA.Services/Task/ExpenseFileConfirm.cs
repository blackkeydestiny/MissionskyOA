using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Task
{
    public class ExpenseFileConfirm : TaskBase, ITaskRunnable
    {
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
                #region 推送消息

                var sql = @"SELECT * FROM ExpenseAuditHistory
                            WHERE [Status]= {0} AND [CurrentAudit]={1} 
	                        ;";

                //推送消息
                var notification = new NotificationModel()
                {
                    CreatedUserId = 0,
                    //MessageType = NotificationType.PushMessage,
                    MessageType = NotificationType.Email,
                    BusinessType = BusinessType.Approving,
                    Title = "及时提交报销申请资料",
                    MessagePrams = "test",
                    Scope = NotificationScope.User,
                    CreatedTime = DateTime.Now
                };
                var financialUserEntity = dbContext.Users.FirstOrDefault(it => it.Email != null && it.Email.ToLower().Contains(Global.FinancialEmail));
                var entities =
                    dbContext.ExpenseAuditHistories.SqlQuery(string.Format(sql, (int)OrderStatus.Approved, financialUserEntity.Id)).ToList(); //当前需要提交报销纸质资料的的报销
                var users = dbContext.Users.ToList(); //所有用户
                var content = string.Empty; //详细内容

                entities.ForEach(expenseItem =>
                {
                    var user = users.FirstOrDefault(it => it.Id == expenseItem.ExpenseMain.ApplyUserId);

                    if (user != null)
                    {
                        notification.Target = user.Email;
                        notification.TargetUserIds = new List<int> { user.Id };
                        notification.MessageContent = "您的报销已经成功审批，请及时到财务提交报销申请纸质资料。";
                        NotificationService.Add(notification, Global.IsProduction); //消息推送

                        content = string.Format("{0},{1},报销纸质资料)", content, user.EnglishName);
                    }

                });

                if (!string.IsNullOrEmpty(content)) //详细内容
                {
                    content = content.Substring(1);
                }
                #endregion

                //更新定时间任务
                ScheduledTaskService.UpdateTask(dbContext, task.Id, executeDate, content);

                //更新数据库
                dbContext.SaveChanges();

                //Log.Info(string.Format("定时任务执行成功: {0}。", task.Name));
            }
        }
    }
}
