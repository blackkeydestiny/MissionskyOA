using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;

namespace MissionskyOA.Services
{
    /// <summary>
    /// User Interface
    /// </summary>
    public partial interface INotificationService
    {

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isProduction">IOS消息推送环境是否是产品环境</param>
        /// <returns></returns>
        bool Add(NotificationModel model, bool isProduction = false);

        /// <summary>
        /// 获取单个消息明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationModel GetNotificationById(int id);

        /// <summary>
        /// 获取用户的消息列表
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<NotificationModel> GetMyNotifications(int currentUserId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取消息列表(后台管理)
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        ListResult<NotificationModel> List(int pageNo, int pageSize, SortModel sort);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

    }
}
