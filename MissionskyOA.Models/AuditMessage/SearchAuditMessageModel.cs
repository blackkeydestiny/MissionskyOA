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
    /// 获取审计信息模型
    /// </summary>
    public class SearchAuditMessageModel 
    {
        /// <summary>
        /// 审计信息类型
        /// </summary>
        [Description("审计信息类型")]
        public AuditMessageType Type { get; set; }

        /// <summary>
        /// 接收用户ID
        /// </summary>
        [Description("接收用户ID")]
        public int UserId { get; set; }

        /// <summary>
        /// 审计状态状态
        /// </summary>
        [Description("审计信息状态")]
        public AuditMessageStatus Status { get; set; }
    }
}
