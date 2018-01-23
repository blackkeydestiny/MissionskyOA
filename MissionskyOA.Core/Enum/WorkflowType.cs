using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 流程类型
    /// </summary>
    public enum WorkflowType
    {
        /// <summary>
        /// 无类型
        /// </summary>
        [Description("无类型")]
        None = 0,

        /// <summary>
        /// 请假流程
        /// </summary>
        [Description("请假流程")]
        AskLeave = 1,

        /// <summary>
        /// 加班流程
        /// </summary>
        [Description("加班流程")]
        Overtime = 2
    }
}
