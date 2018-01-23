using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
namespace MissionskyOA.Services
{
    public partial interface IAnnouncementService
    {
        /// <summary>
        /// 申请通告
        /// </summary>
        /// <param name="AnnoucementModel">通告信息</param>
        /// <returns></returns>
        AnnouncementModel AddAnnouncement(AnnouncementModel model);
        
        /// <summary>
        /// 用户是否有操作此通告的权利
        /// </summary>
        /// <param name="AnnoucementModel"></param>
        /// <returns></returns>
        bool isUserHaveAuthority2Announcement(AnnouncementType type, int roleId);

         /// <summary>
        /// 获取待处理的通告申请单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>通告申请单审批列表</returns>
        List<AnnouncementModel> GetPendingAnnocument();

        /// <summary>
        /// 审批通告
        /// </summary>
        /// <param name="model">审批model</param>
        /// <param name="userId">用户Id</param>
        /// <returns>审批通告成功或者失败</returns>
        bool Audit(AuditAnnouncementModel model, int userId, string userName);

        /// 显示在有效期内的通告
        /// </summary>
        /// <returns>有效期内的通告信息</returns>
        IPagedList<AnnouncementModel> AnnouncementList(int pageIndex, int pageSize);

        /// 显示所有的通告
        /// </summary>
        /// <returns>显示所有的通告</returns>
        ListResult<AnnouncementModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 根据ID获取通告
        /// </summary>
        /// <param name="Id">通告Id</param>
        /// <returns></returns>
        AnnouncementModel GetAnnouncementByID(int id);
    }
}
