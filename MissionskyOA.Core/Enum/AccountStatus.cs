using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 账号状态
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 0,

        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Lock = 1,

        /// <summary>
        /// 重置密码
        /// </summary>
        [Description("重置密码")]
        RestPassword = 2
    }
}
