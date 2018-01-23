using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 盘底任务状态
    /// </summary>
    public enum AssetInventoryStatus
    {
        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        Closed = 0,

        /// <summary>
        /// 开启中
        /// </summary>
        [Description("开启")]
        Open = 1,
    }
}
