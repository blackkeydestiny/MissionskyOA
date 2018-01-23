using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 书籍状态
    /// </summary>
    public enum BookSource
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        None = 0,

        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        Purchase = 1,

        /// <summary>
        /// 捐赠
        /// </summary>
        [Description("捐赠")]
        Donate = 2
    }
}
