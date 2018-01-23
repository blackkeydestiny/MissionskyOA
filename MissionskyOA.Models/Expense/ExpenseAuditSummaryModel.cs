using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ExpenseAuditSummaryModel
    {
        [Description("当前用对此报销单的审批状态")]
        public OrderStatus Status { get; set; }

        [Description("当前用户是否是已审批")]
        public bool IsAudited { get; set; }

        [Description("报销单信息")]
        public ExpenseMainModel model { get; set; }
    }
}
