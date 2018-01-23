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
    /// 会议通知
    /// </summary>
    public class MeetingNotificationModel
    {
        /// <summary>
        /// 会议Id
        /// </summary>
        [Description("会议Id")]
        public int Meeting { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        [Description("通知时间")]
        public DateTime NotifiedTime { get; set; }

        /// <summary>
        /// 会议通知类型
        /// </summary>
        [Description("会议通知类型")]
        public MeetingNotificationType Type { get; set; }
    }


    /// <summary>
    /// 会议通知类型(会议开始通知，会议结束通知)
    /// </summary>
    public enum MeetingNotificationType
    {
        /// <summary>
        /// 无效类型
        /// </summary>
        [Description("无效类型")]
        None = 0,
        
        /// <summary>
        /// 会议开始通知
        /// </summary>
        [Description("会议开始通知")]
        StartNotification = 1,
        
        /// <summary>
        /// 会议结束通知
        /// </summary>
        [Description("会议结束通知")]
        EndNotification = 2
    }
}
