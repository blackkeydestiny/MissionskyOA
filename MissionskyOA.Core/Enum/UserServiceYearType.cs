using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 员工的总共的服务年限（工龄）
    /// </summary>
    public enum UserServiceYearType
    {
        /// <summary>
        /// 工龄在10年以下，不包括10年
        /// </summary>
        [Description("TenYears")]
        TenYears = 1,

        /// <summary>
        /// 工龄在10年到20年之间，不包括20年
        /// </summary>
        [Description("TwentyYears")]
        TwentyYears = 2,

        /// <summary>
        /// 工龄在20年以上
        /// </summary>
        [Description("ThirtyYears")]
        ThirtyYears = 3,
    }
}
