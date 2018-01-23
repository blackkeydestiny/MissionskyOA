using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core
{
    /// <summary>
    /// 工作任务类型
    /// </summary>
    public enum WorkTaskType
    {
        /// <summary>
        /// 无效类型
        /// </summary>
        [Description("无效类型")]
        Invalid = 0,

        /// <summary>
        /// 检查
        /// </summary>
        [Description("检查")]
        Inspection = 1,

        /// <summary>
        /// 执行
        /// </summary>
        [Description("执行")]
        Execute = 2,

        /// <summary>
        /// 通知
        /// </summary>
        [Description("通知")]
        Notice = 3,
    }
}
