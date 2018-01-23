using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ApplyExpenseDetailModel
    {
        [Description("报销活动日期")]
        public DateTime ODate { get; set; }
        [Description("报销类型")]
        public ExpenseType EType { get; set; }
        [Description("理由")]
        public string Remark { get; set; }
        [Description("数量")]
        public virtual int PCount { get; set; }
        [Description("金额")]
        public virtual decimal Amount { get; set; }
        [Description("报销类型")]
        public ExpenseType ExpenseType { get; set; }
        [Description("参与人员")]
        public virtual int[] participants { get; set; }
    }
}
