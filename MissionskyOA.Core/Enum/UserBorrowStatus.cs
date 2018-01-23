using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 用户借阅状态
    /// </summary>
    public enum UserBorrowStatus
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        [Description("无效状态")]
        None = 0,

        /// <summary>
        /// 借阅
        /// </summary>
        [Description("借阅")]
        Borrowing = 1,

        /// <summary>
        /// 续借
        /// </summary>
        [Description("续借")]
        Renewed = 2,

        /// <summary>
        /// 归还
        /// </summary>
        [Description("归还")]
        Returned = 3,

        /// <summary>
        /// 遗失
        /// </summary>
        [Description("遗失")]
        Lost = 4
    }
}
