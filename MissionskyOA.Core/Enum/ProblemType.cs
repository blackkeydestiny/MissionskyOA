using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 意见反馈(问题类型)
    /// </summary>
    public enum ProblemType
    {
        /// <summary>
        /// 建议
        /// </summary>
        [Description("建议")]
        Invalid = 1,

        [Description("问题")]
        Inspection = 2
    }
}
