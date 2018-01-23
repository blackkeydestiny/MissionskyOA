using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 流程经办明细模型
    /// </summary>
    public class WorkflowProcessModel : BaseModel
    {
        [Description("批量申请Id")]
        public int OrderNo { get; set; }

        [Description("流程Id")]
        public int FlowId { get; set; }
        
        [Description("步骤Id")]
        public int StepId { get; set; }

        [Description("步骤名称")]
        public string StepName { get; set; }

        [Description("经办人Id")]
        public int Operator { get; set; }

        [Description("经办人姓名")]
        public string OperatorName { get; set; }

        [Description("处理类型")]
        public WorkflowOperation Operation { get; set; }

        [Description("处理说明")]
        public string OperationDesc { get; set; }

        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }

        [Description("下一个审批步骤")]
        public WorkflowStepModel NextStep { get; set; }

        [Description("下一个审批人")]
        public UserModel NextApprover { get; set; }

        [Description("申请单转换状态")]
        public OrderStatus NextStatus;
    }
}
