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
    /// <summary>
    /// 通知用户归还图书
    /// </summary>
    public class NotifyReturnBook : TaskBase, ITaskRunnable
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

                var sql = @"SELECT * FROM BookBorrow
                            WHERE ISNULL([Status], 0) = {0} --借阅中
                            AND DATEDIFF(DAY, ReturnDate, GETDATE()) >= 0 --已到预计归还日期
	                        ;";

                //推送消息
                var notification = new NotificationModel()
                {
                    CreatedUserId = 0,
                    //MessageType = NotificationType.PushMessage,
                    MessageType = NotificationType.Email,
                    BusinessType = BusinessType.Approving,
                    Title = "用户归还图书通知",
                    MessagePrams = "test",
                    Scope = NotificationScope.User,
                    CreatedTime = DateTime.Now
                };

                var entities =
                    dbContext.BookBorrows.SqlQuery(string.Format(sql, (int)UserBorrowStatus.Borrowing)).ToList(); //当前需要归还的借阅图书
                var users = dbContext.Users.ToList(); //所有用户
                var content = string.Empty; //详细内容

                entities.ForEach(borrow =>
                {
                    var user = users.FirstOrDefault(it => it.Id == borrow.UserId);
                    var book = dbContext.Books.FirstOrDefault(it => it.Id == borrow.BookId);

                    if (user != null && book != null)
                    {
                        notification.Target = user.Email;
                        notification.TargetUserIds = new List<int> {user.Id};
                        notification.MessageContent = string.Format("请及时归还您借阅的图书《{0}》。", book.Name);
                        NotificationService.Add(notification, Global.IsProduction); //消息推送

                        content = string.Format("{0},{1}({2})", content, user.EnglishName, book.Name);
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
