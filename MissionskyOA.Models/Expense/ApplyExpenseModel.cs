using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ApplyExpenseModel
    {
        [Description("部门ID")]
        public int DeptNo { get; set; }
        [Description("项目组ID")]
        public int ProjNo { get; set; }
        [Description("总金额")]
        public decimal Amount { get; set; }
        [Description("申请理由")]
        public string Reason { get; set; }
        [Description("报销具体信息")]
        public List<ApplyExpenseDetailModel> ExpenseDetails { get; set; }
    }
}
