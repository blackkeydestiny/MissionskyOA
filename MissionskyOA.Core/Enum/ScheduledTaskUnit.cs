using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 定时任务时间间隔单位
    /// </summary>
    public enum ScheduledTaskUnit
    {
        /// <summary>
        /// 无效单位
        /// </summary>
        [Description("无效单位")]
        None = 0,

        /// <summary>
        /// 秒
        /// </summary>
        [Description("秒")]
        Second = 1,

        /// <summary>
        /// 分
        /// </summary>
        [Description("分")]
        Minute = 2,

        /// <summary>
        /// 时
        /// </summary>
        [Description("时")]
        Hour = 3,

        /// <summary>
        /// 天
        /// </summary>
        [Description("天")]
        Day = 4,

        /// <summary>
        /// 月
        /// </summary>
        [Description("月")]
        Month = 5
    }
}
