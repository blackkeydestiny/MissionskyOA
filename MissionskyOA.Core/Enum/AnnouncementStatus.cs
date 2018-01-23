using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 公告状态
    /// </summary>
    public enum AnnouncementStatus
    {
        /// <summary>
        /// 申请
        /// </summary>
        [Description("申请")]
        Apply = 1,

        /// <summary>
        /// 已批准
        /// </summary>
        [Description("允许发布")]
        AllowPublish = 2,

        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("拒绝发布")]
        RejectPublish = 3
    }
}
