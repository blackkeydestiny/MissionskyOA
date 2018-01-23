using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Task
{
    /// <summary>
    /// 定时任务接口
    /// </summary>
    public interface ITaskRunnable
    {
        /// <summary>
        /// 执行定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <param name="executeDate">执行时间</param>
        void Execute(ScheduledTaskModel task, DateTime executeDate);
    }
}
