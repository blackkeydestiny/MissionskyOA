using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 通告
    /// </summary>
    [Description("通告")]
    public class AnnouncementModel : BaseModel
    {
        /// <summary>
        /// 通告类型
        /// </summary>
        [Description("通告类型")]
        public AnnouncementType Type { get; set; }
        /// <summary>
        /// 通告标题
        /// </summary>
        [Description("通告标题")]
        public string Title { get; set; }
        /// <summary>
        /// 通告内容
        /// </summary>
        [Description("通告内容")]
        public string Content { get; set; }

        /// <summary>
        /// 申请用户ID
        /// </summary>
        [Description("申请用户ID")]
        public int ApplyUserId { get; set; }

        /// <summary>
        /// 通告状态
        /// </summary>
        [Description("通告状态")]
        public AnnouncementStatus Status { get; set; }

        /// <summary>
        /// 审批通告
        /// </summary>
        [Description("审批理由")]
        public string AuditReason { get; set; }

        /// <summary>
        /// 申请用户名
        /// </summary>
        [Description("申请用户名")]
        public string ApplyUserName { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        [Description("有效天数")]
        public int? EffectiveDays { get; set; }
        /// <summary>
        /// 创建用户Id
        /// </summary>
        [Description("创建用户Id")]
        public int? CreateUserId { get; set; }
        /// <summary>
        /// 创建用户名
        /// </summary>
        [Description("创建用户名")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建用户名
        /// </summary>
        [Description("关联资产盘点Id")]
        public int? RefAssetInventoryId { get; set; }

        /// <summary>
        /// 创建用户名
        /// </summary>
        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }
    }
}
