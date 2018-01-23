using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    public class OperateOrderModel
    {
        [Description("申请单号")]
        public int OrderNo { get; set; }

        [Description("审批意见")]
        public string Opinion { get; set; }
        
        [Description("流程处理类型")]
        public WorkflowOperation Operation { get; set; }
    }
}
