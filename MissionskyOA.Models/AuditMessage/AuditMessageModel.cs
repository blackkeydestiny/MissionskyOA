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
    /// 审计信息模型
    /// </summary>
    public class AuditMessageModel : BaseModel
    {
        /// <summary>
        /// 审计信息类型
        /// </summary>
        [Description("审计信息类型")]
        public AuditMessageType Type { get; set; }

        /// <summary>
        /// 审计信息
        /// </summary>
        [Description("审计信息")]
        public string Message { get; set; }

        /// <summary>
        /// 接收用户ID
        /// </summary>
        [Description("接收用户ID")]
        public int UserId { get; set; }

        [Description("接收用户名")]
        public string UserEnglishName { get; set; }


        /// <summary>
        /// 审计状态状态
        /// </summary>
        [Description("审计信息状态")]
        public AuditMessageStatus Status { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Description("创建日期")]
        public DateTime CreatedTime { get; set; }
    }
}
