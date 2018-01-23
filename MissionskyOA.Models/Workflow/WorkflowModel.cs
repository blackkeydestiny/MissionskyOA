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
    /// 流程模型
    /// </summary>
    public class WorkflowModel : BaseModel
    {
        [Description("流程名称")]
        public string Name { get; set; }

        [Description("流程描述")]
        public string Desc { get; set; }

        [Description("流程状态")]
        public WorkflowStatus Status { get; set; }

        [Description("流程类型")]
        public WorkflowType Type { get; set; }
        
        /// <summary>
        /// 流程申请人条件
        /// </summary>
        [Description("流程申请人条件")]
        public string Condition { get; set; }

        /// <summary>
        /// 流程申请人条件说明
        /// </summary>
        [Description("流程申请人条件说明")]
        public string ConditionDesc { get; set; }

        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }

        [Description("流程步骤")]
        public List<WorkflowStepModel> WorkflowSteps { get; set; }
    }
}
