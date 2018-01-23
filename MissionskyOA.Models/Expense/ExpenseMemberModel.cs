using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ExpenseMemberModel : BaseModel
    {
        [Description("报销具体信息id")]
        public int DId { get; set; }
        [Description("报销参与人员id")]
        public int MemberId { get; set; }

        public virtual ExpenseDetailModel ExpenseDetail { get; set; }
        public virtual UserModel User { get; set; }
    }
}
