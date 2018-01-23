using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 业务类型
    /// </summary>
    [Description("业务类型")]
    public enum BusinessType
    {
        /// <summary>
        /// 审批通知
        /// </summary>
        [Description("审批通知")]
        Approving = 0,

        /// <summary>
        /// 预定会议室成功
        /// </summary>
        [Description("预定会议室成功")]
        BookMeetingRoomSuccessAnnounce = 1,

        /// <summary>
        ///取消会议室预定成功
        /// </summary>
        [Description("取消会议室预定成功")]
        CacnelMeetingRoomSuccessAnnounce = 2,

        /// <summary>
        /// 行政部门公告通知
        /// </summary>
        [Description("行政部门公告通知")]
        AdministrationEventAnnounce = 3,

        /// <summary>
        /// 快递消息
        /// </summary>
        [Description("快递消息")]
        ExpressMessage=4,

        /// <summary>
        /// 资产转移
        /// </summary>
        [Description("资产转移")]
        AssetTransfer=5,

        /// <summary>
        /// 资产盘点
        /// </summary>
        [Description("资产盘点")]
        AssetInventory=6,


        /// <summary>
        /// 通知用户归还图书
        /// </summary>
        [Description("通知用户归还图书")]
        NotifyReturnBook = 7,

        /// <summary>
        /// 通知会议开始
        /// </summary>
        [Description("通知会议开始")]
        NotifyMeetingStart = 8,

        /// <summary>
        /// 通知会议结束
        /// </summary>
        [Description("通知会议结束")]
        NotifyMeetingEnd = 9,
		
		/// <summary>
        /// 快递消息
        /// </summary>
        [Description("通告审批消息")]
        AnnocumentAuditMessage=10

    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Email
        /// </summary>
        Email = 0,

        /// <summary>
        /// 短信
        /// </summary>
        SMessage = 1,

        /// <summary>
        /// 推送消息
        /// </summary>
        PushMessage = 2,

        // <summary>
        /// 通告
        /// </summary>
        PublicMessage = 3

    }

    /// <summary>
    /// 消息范围
    /// </summary>
    public enum NotificationScope
    {
        /// <summary>
        /// 公开
        /// </summary>
        Public = 0,

        /// <summary>
        ///用户
        /// </summary>
        User = 1,

        /// <summary>
        /// 用户组
        /// </summary>
        UserGroup = 2
    }

}
