using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core
{
    /// <summary>
    /// 工作任务状态
    /// </summary>
    public enum WorkTaskStatus
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        [Description("无效状态")]
        Invalid = 0,

        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Created = 1,

        /// <summary>
        /// 执行
        /// </summary>
        [Description("执行")]
        Started = 2,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Completed = 3,

        /// <summary>
        /// 暂停
        /// </summary>
        [Description("暂停")]
        Paused = 4,
        
        /// <summary>
        /// 放弃
        /// </summary>
        [Description("放弃")]
        GivenUp = 5,
        
        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Closed = 6
    }
}
