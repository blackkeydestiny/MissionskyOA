using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class ScheduledTaskModel : BaseModel
    {
        /// <summary>
        /// 定时任务名称
        /// </summary>
        [Description("定时任务名称")]
        public string Name { get; set; }

        /// <summary>
        /// 执行时间间隔
        /// </summary>
        [Description("执行时间间隔")]
        public int Interval { get; set; }

        /// <summary>
        /// 间隔单位
        /// </summary>
        [Description("间隔单位")]
        public ScheduledTaskUnit Unit { get; set; }

        /// <summary>
        /// 定时任务状态
        /// </summary>
        [Description("定时任务状态")]
        public ScheduledTaskStatus Status { get; set; }

        /// <summary>
        /// 任务处理类
        /// </summary>
        [Description("任务处理类")]
        public string TaskClass { get; set; }

        /// <summary>
        /// 定时任务说明
        /// </summary>
        [Description("定时任务说明")]
        public string Desc { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        [Description("上次执行时间")]
        public DateTime? LastExecTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }
    }
}
