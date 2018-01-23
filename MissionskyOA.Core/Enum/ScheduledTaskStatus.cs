using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 定时任务状态
    /// </summary>
    public enum ScheduledTaskStatus
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        [Description("无效状态")]
        None = 0,

        /// <summary>
        /// 启动
        /// </summary>
        [Description("启动")]
        Started = 1,

        /// <summary>
        /// 停止
        /// </summary>
        [Description("停止")]
        Stopped = 2
    }
}
