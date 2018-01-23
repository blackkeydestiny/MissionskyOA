using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class UpdateExpenseModel : BaseModel
    {
        [Description("项目组")]
        public virtual int ProjNo { get; set; }
        [Description("金额")]
        public virtual decimal Amount { get; set; }
        
        [Description("理由或备注")]
        public string Reason { get; set; }
    }
}
