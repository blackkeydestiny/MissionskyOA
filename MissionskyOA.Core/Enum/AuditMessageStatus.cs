using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 申请消息状态
    /// </summary>
    public enum AuditMessageStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        [Description("无状态")]
        None = 0,

        /// <summary>
        /// 未读
        /// </summary>
        [Description("未读")]
        Unread = 1,
        
        /// <summary>
        /// 已读
        /// </summary>
        [Description("已读")]
        Read = 2
    }
}
