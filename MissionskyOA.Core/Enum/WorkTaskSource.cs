using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core
{
    /// <summary>
    /// 工作任务来源
    /// </summary>
    public enum WorkTaskSource
    {
        /// <summary>
        /// 无效来源
        /// </summary>
        [Description("无效来源")]
        Invalid = 0,

        /// <summary>
        /// 会议
        /// </summary>
        [Description("会议")]
        Meeting = 1,

        /// <summary>
        /// 通讯录个人
        /// </summary>
        [Description("通讯录个人")]
        Person = 2,
    }
}
