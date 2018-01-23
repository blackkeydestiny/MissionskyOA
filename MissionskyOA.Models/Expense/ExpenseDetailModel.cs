using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ExpenseDetailModel:BaseModel
    {
        [Description("报销单id")]
        public int MId { get; set; }

        [Description("报销活动日期")]
        public DateTime ODate { get; set; }

        [Description("报销类型")]
        public ExpenseType EType { get; set; }

        [Description("理由或备注")]
        public string Remark { get; set; }

        [Description("数量")]
        public int PCount { get; set; }

        [Description("金额")]
        public decimal Amount { get; set; }

        public virtual string ExpenseMemberNames { get; set; }

        public virtual ExpenseMainModel ExpenseMain { get; set; }

        [Description("报销类型")]
        public ExpenseType ExpenseType { get; set; }
        public virtual List<ExpenseMemberModel> ExpenseMembers { get; set; }
    }
}
