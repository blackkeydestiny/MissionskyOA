using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Task
{
    /// <summary>
    /// 统计今日员工状态
    /// </summary>
    public class SummaryTodayStatus : TaskBase, ITaskRunnable
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
                var sql = @"IF EXISTS(SELECT 1 FROM tempdb..SYSOBJECTS WHERE Id = OBJECT_ID('tempdb..##TUser'))
								DROP TABLE ##TUser
								
							SELECT DISTINCT O.UserId INTO ##TUser FROM [Order] O 
								INNER JOIN OrderDet D ON O.Id = D.OrderId
								--INNER JOIN [User] U ON O.UserId = U.Id AND ISNULL(U.TodayStatus, 1) = 1
		                        WHERE DATEDIFF(DAY, CAST(StartDate AS NVARCHAR) + ' ' + CAST(StartTime AS NVARCHAR), GETDATE()) >= 0 
			                        AND DATEDIFF(DAY, CAST(EndDate AS NVARCHAR) + ' ' + CAST(EndTime AS NVARCHAR), GETDATE()) <= 0 --今天请假
			                        AND ISNULL(O.OrderType, 0) IN (1, 2, 3, 5, 6, 7, 8) --年假，病假，调休，婚假，产假，丧假，事假

							
							SELECT ',' + EnglishName FROM [User] WHERE Id IN (SELECT UserId FROM ##TUser) AND ISNULL(TodayStatus, 1) != 3 FOR XML PATH (''); --需要更新今日状态
	                        UPDATE [User] SET TodayStatus = 3 WHERE Id IN (SELECT UserId FROM ##TUser) AND ISNULL(TodayStatus, 1) != 3; --更新今日状态为请假状态		
                            UPDATE [User] SET TodayStatus = 1 WHERE Id NOT IN (SELECT UserId FROM ##TUser) AND ISNULL(TodayStatus, 1) = 3; --请假状态改为正常状态
	                        

							IF EXISTS(SELECT 1 FROM tempdb..SYSOBJECTS WHERE Id = OBJECT_ID('tempdb..##TUser'))
								DROP TABLE ##TUser";

                //执行定时间任务
                var content = string.Empty; //详细内容
                var data = dbContext.Database.SqlQuery<string>(sql);

                if (data != null)
                {
                    content = data.FirstOrDefault();

                    if (!string.IsNullOrEmpty(content))
                    {
                        content = content.Substring(1);
                    }
                }

                //更新定时间任务
                ScheduledTaskService.UpdateTask(dbContext, task.Id, executeDate, content);

                //更新数据库
                dbContext.SaveChanges();

                //Log.Info(string.Format("定时任务执行成功: {0}。", task.Name));
            }
        }
    }
}
