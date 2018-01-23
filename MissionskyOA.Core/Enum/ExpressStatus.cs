using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 快递状态
    /// </summary>
    public enum ExpressStatus
    {
        /// <summary>
        ///代签
        /// </summary>
        [Description("代签")]
        Proxy = 0,

        /// <summary>
        ///接收
        /// </summary>
        [Description("接收")]
        Accepted = 1,

        /// <summary>
        /// 退回
        /// </summary>
        [Description("退回")]
        Reject = 2,

        /// <summary>
        /// 退回
        /// </summary>
        [Description("无效")]
        Invalid = 3
    }
}
