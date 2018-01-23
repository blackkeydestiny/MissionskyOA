using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 流程处理人类型
    /// </summary>
    public enum WorkflowOperator
    {
        /// <summary>
        /// 无审批人
        /// </summary>
        [Description("无审批人")]
        None = 0,

        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        Role = 1,

        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        User = 2
    }
}
