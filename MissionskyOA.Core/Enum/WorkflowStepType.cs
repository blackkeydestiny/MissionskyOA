using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 流程步骤类型
    /// </summary>
    public enum WorkflowStepType
    {
        /// <summary>
        /// 用户申请
        /// </summary>
        [Description("用户申请")]
        UserApply = 0,

        /// <summary>
        /// 无类型
        /// </summary>
        [Description("无类型")]
        None = 0,

        /// <summary>
        /// 领导审批
        /// </summary>
        [Description("领导审批")]
        LeaderApprove = 1,

        /// <summary>
        /// 行政审批
        /// </summary>
        [Description("行政审批")]
        AdminReview = 2,

        /// <summary>
        /// 财务审批
        /// </summary>
        [Description("财务审批")]
        FinanceReview = 3
    }
}
