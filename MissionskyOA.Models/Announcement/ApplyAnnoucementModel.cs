using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{    /// <summary>
    /// 申请通告
    /// </summary>
    [Description("申请通告")]
    public class ApplyAnnoucementModel : BaseModel
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
        [Description("申请用户ID,可选参数")]
        public virtual int ApplyUserId { get; set; }

        /// <summary>
        /// 有效天数
        /// </summary>
        [Description("有效天数")]
        public int? EffectiveDays { get; set; }

    }
}


