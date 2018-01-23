using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 申请消息类型
    /// </summary>
    public enum AuditMessageType
    {
        /// <summary>
        /// 无指定类型
        /// </summary>
        [Description("无指定类型")]
        None = 0,

        /// <summary>
        /// 申请加班提交成功消息
        /// </summary>
        [Description("申请加班提交成功消息")]
        Apply_OT_Application_Message = 1,
        
        /// <summary>
        /// 申请加班获批消息
        /// </summary>
        [Description("申请加班获批消息")]
        Approve_OT_Application_Message = 2,
        
        /// <summary>
        /// 申请加班被拒消息
        /// </summary>
        [Description("申请加班被拒消息")]
        Recject_OT_Application_Message = 3,

        /// <summary>
        /// 申请撤销加班提交成功消息
        /// </summary>
        [Description("申请撤销加班提交成功消息")]
        Apply_OT_Cancel_Application_Message = 4,

        /// <summary>
        /// 申请撤销加班获批消息
        /// </summary>
        [Description("申请撤销加班获批消息")]
        Approve_OT_Cance_Application_Message = 5,

        /// <summary>
        /// 申请请假提交成功消息
        /// </summary>
        [Description("申请请假提交成功消息")]
        Apply_Leave_Application_Message = 6,

        /// <summary>
        /// 申请请假获批消息
        /// </summary>
        [Description("申请请假获批消息")]
        Approve_Leave_Application_Message = 7,

        /// <summary>
        /// 申请请假被拒消息
        /// </summary>
        [Description("申请请假被拒消息")]
        Recject_Leave_Application_Message = 8,

        /// <summary>
        /// 申请撤销请假提交成功消息
        /// </summary>
        [Description("申请撤销请假提交成功消息")]
        Apply_Leave_Cancel_Application_Message = 9,

        /// <summary>
        /// 申请撤销请假获批消息
        /// </summary>
        [Description("申请撤销请假获批消息")]
        Approve_Leave_Cacnel_Application_Message = 10,

        /// <summary>
        /// 假期调整获批成功消息
        /// </summary>
        [Description("假期调整获批成功消息")]
        Approve_Annual_Vacation_Roll_Application_Message = 11,

        /// <summary>
        /// 申请报销获批消息
        /// </summary>
        [Description("申请经费获批消息")]
        Approve_Expense_Application_Message = 12,

        /// <summary>
        /// 申请报销被拒消息
        /// </summary>
        [Description("申请经费被拒消息")]
        Recject_Expense_Application_Message = 13,

        /// <summary>
        /// 申请撤销报销提交成功消息
        /// </summary>
        [Description("申请撤销报销提交成功消息")]
        Apply_Expense_Cancel_Application_Message = 14,

        /// <summary>
        /// 申请报销被拒消息
        /// </summary>
        [Description("申请经费纸质资料确认消息")]
        Confirm_Expense_File_Message = 15,
        
        #region 定时任务 (大于200，小于300)
        /// <summary>
        /// 统计今日请假状态
        /// </summary>
        [Description("统计今日请假状态")]
        Task_Summary_Today_Status = 201,

        /// <summary>
        /// 提醒用户归还图书
        /// </summary>
        [Description("提醒用户归还图书")]
        Task_Notice_Return_Book = 202
        #endregion
    }
}
