using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ExpenseAuditHistoryModel : BaseModel
    {
        [Description("当前审批人")]
        public int CurrentAudit { get; set; }
        [Description("下一步审批人")]
        public int? NextAudit { get; set; }
        [Description("报销单id")]
        public int ExpenseId { get; set; }
        [Description("审批状态")]
        public OrderStatus? Status { get; set; }
        [Description("审批信息")]
        public string AuditMessage { get; set; }
         [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }
         [Description("当前审批人姓名")]
        public string CurrentAuditName { get; set; }
         [Description("下一步审批人姓名")]
        public string NextAuditName { get; set; }
        public virtual ExpenseMainModel ExpenseMain { get; set; }
    }
}
