using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Task
{
    /// <summary>
    /// 定时任务父类型
    /// </summary>
    public class TaskBase
    {
        /// <summary>
        /// 定时任务Service
        /// </summary>
        protected static readonly IScheduledTaskService ScheduledTaskService = new ScheduledTaskService();
        
        /// <summary>
        /// 消息推送Service
        /// </summary>
        protected readonly INotificationService NotificationService = new NotificationService();

        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(TaskBase));
    }
}
