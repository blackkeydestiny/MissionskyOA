using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 今日状态
    /// </summary>
    public enum UserTodayStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        [Description("无状态")]
        None = 0,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,

        /// <summary>
        /// 出差
        /// </summary>
        [Description("出差")]
        BusinessTrip = 2,

        /// <summary>
        /// 请假
        /// </summary>
        [Description("请假")]
        AskLeave = 3
    }
}
