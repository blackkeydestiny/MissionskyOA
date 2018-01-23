using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Task
{
    /// <summary>
    /// 统计考勤详细
    /// </summary>
    public class SummaryAttendanceDetail : TaskBase, ITaskRunnable
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
                DateTime summaryDate = DateTime.Now.Date; //统计日期

                //统计当前日期后3个月的考勤详细
                do
                {
                    var parameters = new []
                    {
                        new SqlParameter()
                        {
                            DbType = System.Data.DbType.Date,
                            ParameterName = "@date",
                            Value = summaryDate
                        }
                    };

                    dbContext.Database.ExecuteSqlCommand("exec p_summaryAttendanceDetail @date", parameters);

                    summaryDate = summaryDate.AddMonths(-1); //统计成功
                } while (((DateTime.Now.Year - summaryDate.Year)*12 + DateTime.Now.Month) - summaryDate.Month < 3);
                
                //更新定时间任务
                ScheduledTaskService.UpdateTask(dbContext, task.Id, executeDate, string.Format("统计{0}-{1}及前2个月的考勤详细成功。", DateTime.Now.Year, DateTime.Now.Month));

                //更新数据库
                dbContext.SaveChanges();

                //Log.Info(string.Format("定时任务执行成功: {0}。", task.Name));
            }
        }
    }
}
