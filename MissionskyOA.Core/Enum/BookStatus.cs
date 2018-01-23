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
    public enum BookStatus
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        [Description("无效状态")]
        None = 0,

        /// <summary>
        /// 在库
        /// </summary>
        [Description("在库")]
        Stored = 1,

        /// <summary>
        /// 借阅
        /// </summary>
        [Description("借阅")]
        Borrowed = 2,

        /// <summary>
        /// 遗失
        /// </summary>
        [Description("遗失")]
        Lost = 3,

        /// <summary>
        /// 下架
        /// </summary>
        [Description("下架")]
        Removed = 4
    }
}
