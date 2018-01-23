using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core
{
    /// <summary>
    /// 工作任务优先级
    /// </summary>
    public enum WorkTaskPriority
    {
        /// <summary>
        /// 无效级别
        /// </summary>
        [Description("无效级别")]
        Invalid = 0,

        /// <summary>
        /// 一般
        /// </summary>
        [Description("一般")]
        Normal = 1,

        /// <summary>
        /// 重要
        /// </summary>
        [Description("重要")]
        Important = 2,

        /// <summary>
        /// 紧急
        /// </summary>
        [Description("紧急")]
        Critical = 3
    }

    /// <summary>
    /// 紧急性
    /// </summary>
    public enum WorkTaskUrgency
    {
        /// <summary>
        /// 无效级别
        /// </summary>
        [Description("无效级别")]
        Invalid = 0,

        /// <summary>
        /// 高
        /// </summary>
        [Description("高")]
        High = 1,

        /// <summary>
        /// 中
        /// </summary>
        [Description("中")]
        Medium = 2,

        /// <summary>
        /// 低
        /// </summary>
        [Description("低")]
        Low = 3
    }

    /// <summary>
    /// 重要性
    /// </summary>
    public enum WorkTaskImportance
    {
        /// <summary>
        /// 无效级别
        /// </summary>
        [Description("无效级别")]
        Invalid = 0,

        /// <summary>
        /// 高
        /// </summary>
        [Description("高")]
        High = 1,

        /// <summary>
        /// 中
        /// </summary>
        [Description("中")]
        Medium = 2,

        /// <summary>
        /// 低
        /// </summary>
        [Description("低")]
        Low = 3
    } 
}
