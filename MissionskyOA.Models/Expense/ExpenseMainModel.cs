using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ExpenseMainModel:BaseModel
    {
        [Description("审批步骤id")]
        public int AuditId { get; set; }
        [Description("部门id")]
        public int DeptNo { get; set; }
        [Description("项目id")]
        public int ProjNo { get; set; }
        [Description("金额")]
        public decimal Amount { get; set; }
        [Description("理由或备注")]
        public string Reason { get; set; }

        [Description("申请人")]
        public int? ApplyUserId { get; set; }

        [Description("生成报销单次数")]
        public int PrintForm { get; set; }

        [Description("确认收到纸质报销单材料")]
        public bool ConfirmForm { get; set; }

        [Description("申请人名")]
        public string ApplyUserName { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }
        [Description("当前审批状态")]
        public ExpenseAuditHistoryModel currentAuditStatus { get; set; }
        public virtual DepartmentModel Department { get; set; }
        public virtual List<ExpenseAuditHistoryModel> ExpenseAuditHistories { get; set; }
        public virtual List<ExpenseDetailModel> ExpenseDetails { get; set; }
        public virtual ProjectModel Project { get; set; }
    }
}
