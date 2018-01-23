using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 申请单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        [Description("无效状态")]
        Invalid = -1,

        /// <summary>
        /// 申请
        /// </summary>
        [Description("申请")]
        Apply = 0,

        /// <summary>
        /// 审核中
        /// </summary>
        [Description("审核中")]
        Approving = 1,

        /// <summary>
        /// 已批准
        /// </summary>
        [Description("已批准")]
        Approved = 2,

        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("已拒绝")]
        Rejected = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 4,

        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        Revoked = 5,

        /// <summary>
        /// 已接收纸质材料
        /// </summary>
        [Description("已接收")]
        Recieved = 6
    }

}
