using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 资产状态
    /// </summary>
    public enum AssetStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 0,

        /// <summary>
        /// 丢失
        /// </summary>
        [Description("丢失")]
        Lose = 1,

        /// <summary>
        /// 报废
        /// </summary>
        [Description("报废")]
        Scrapped = 2,

        /// <summary>
        /// 待转出
        /// </summary>
        [Description("待转出")]
        WaitOut = 3,

        /// <summary>
        /// 待转入
        /// </summary>
        [Description("待转入")]
        WaitIn = 4,

        /// <summary>
        /// 闲置
        /// </summary>
        [Description("闲置")]
        Idle = 5,
    }

    /// <summary>
    /// 资产转移记录状态
    /// </summary>
    public enum AssetTransactionStatus
    {
        [Description("取消")]
        Canceled = 0
    }
}
