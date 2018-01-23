using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 日历类型
    /// </summary>
    public enum CalendarType
    {
        /// <summary>
        /// 请求无效
        /// </summary>
        [Description("请求无效")]
        Invalid = -1,

        /// <summary>
        /// 工作日
        /// </summary>
        [Description("工作日")]
        Workday = 0,

        /// <summary>
        /// 周末休息日
        /// </summary>
        [Description("周末")]
        Weekend = 1,

        /// <summary>
        /// 节假日
        /// </summary>
        [Description("节假日")]
        Holiday = 2
    }
}
