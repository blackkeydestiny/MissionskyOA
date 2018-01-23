using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum WorkflowStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        [Description("无状态")]
        None = 0,

        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disabled = 1,

        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Enabled = 2
    }
}
