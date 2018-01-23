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
    /// 消息通知Model
    /// </summary>
    public class NotificationModel : BaseModel
    {
        [Description("业务类型")]
        public BusinessType BusinessType { get; set; }

        [Description("消息内容")]
        public string MessageContent { get; set; }

        /// <summary>
        /// 目标用户的极光推送别名、标签，或者邮箱(有可能多人)，手机号码
        /// </summary>
        [Description("目标用户的极光推送别名、标签，或者邮箱，手机号码")]
        public string Target { get; set; }

        [Description("消息类型")]
        public NotificationType MessageType { get; set; }

        /// <summary>
        /// 消息标题(可以为空)
        /// </summary>
        [Description("消息标题")]
        public string Title { get; set; }

        /// <summary>
        /// 消息范围
        /// </summary>
        [Description("消息范围")]
        public NotificationScope Scope { get; set; }

        [Description("消息参数")]
        public string MessagePrams { get; set; }

        [Description("创建消息用户Id")]
        public int? CreatedUserId { get; set; }

        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }

        [Description("就收通知的用户Id集合")]
        public List<int> TargetUserIds { get; set; }
    }
}
