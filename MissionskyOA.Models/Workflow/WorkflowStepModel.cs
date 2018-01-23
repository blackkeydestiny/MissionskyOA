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
    /// 流程步骤模型
    /// </summary>
    public class WorkflowStepModel : BaseModel
    {
        [Description("流程Id")]
        public int FlowId { get; set; }

        [Description("流程名称")]
        public string FlowName { get; set; }

        [Description("流程步骤名称")]
        public string Name { get; set; }

        [Description("流程步骤描述")]
        public string Desc { get; set; }

        [Description("允许批准的最小时长")]
        public double MinTimes { get; set; }

        [Description("允许批准的最大时长")]
        public double MaxTimes { get; set; }

        [Description("步骤节点类型")]
        public WorkflowStepType Type { get; set; }

        [Description("下一步")]
        public int? NextStep { get; set; }

        [Description("上一步")]
        public int? PrevStep { get; set; }

        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }

        [Description("审批人")]
        public int Operator { get; set; }

        [Description("审批人类型")]
        public WorkflowOperator OperatorType { get; set; }
    }
}
