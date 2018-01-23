using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 审批通告
    /// </summary>
    public class AuditAnnouncementModel
    {
        /// <summary>
        /// 通告Id
        /// </summary>
        [Description("通告Id")]
        public int announcementID { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        [Description("审批状态,同意为ture,拒绝为false")]
        public bool auditStatus { get; set; }
        /// <summary>
        /// 审批通告
        /// </summary>
        [Description("审批意见")]
        public string AuditReason { get; set; }    
    }
}
