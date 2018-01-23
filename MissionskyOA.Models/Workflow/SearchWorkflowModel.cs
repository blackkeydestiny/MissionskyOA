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
    /// 查找流程模型
    /// </summary>
    public class SearchWorkflowModel
    {
        [Description("流程Id")]
        public int WorkflowId { get; set; }

        [Description("流程名称")]
        public string Name { get; set; }

        [Description("流程类型")]
        public WorkflowType Type { get; set; }

        [Description("流程状态")]
        public WorkflowStatus Status { get; set; }
    }
}
