using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 流程处理
    /// </summary>
    public enum WorkflowOperation
    {
        /// <summary>
        /// 无处理
        /// </summary>
        [Description("无处理")]
        None = 0,

        /// <summary>
        /// 申请
        /// </summary>
        [Description("申请")]
        Apply = 1,

        /// <summary>
        /// 审批
        /// </summary>
        [Description("审批")]
        Approve = 2,

        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Rejecte = 3,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancel = 4,
        
        /// <summary>
        /// 撤销
        /// </summary>
        [Description("撤销")]
        Revoke = 6
    }
}
