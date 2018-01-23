using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 资产属性数据类型
    /// </summary>
    public enum AssetDataType
    {
        /// <summary>
        /// 整数
        /// </summary>
        [Description("整数")]
        Int = 0,

        /// <summary>
        /// 小数
        /// </summary>
        [Description("小数")]
        Double = 1,

        /// <summary>
        /// 字符
        /// </summary>
        [Description("字符")]
        String = 2,

        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        DateTime = 3
    }
}
