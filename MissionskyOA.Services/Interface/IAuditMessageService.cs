using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 审记信息处理接口
    /// </summary>
    public interface IAuditMessageService
    {
        /// <summary>
        /// 获取用户审计信息
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>返回用户审计信息及分页信息</returns>
        IPagedList<AuditMessageModel> GetUserAuditMessages(SearchAuditMessageModel model, int pageIndex, int pageSize);


        /// <summary>
        /// 添加审计信息
        /// </summary>
        /// <param name="dbContext">上下文</param>
        /// <param name="model">审计信息</param>
        /// <returns>是否更新成功</returns>
        bool AddMessage(MissionskyOAEntities dbContext, AuditMessageModel model);


        /// <summary>
        /// 添加审计信息
        /// </summary>
        /// <param name="model">审计信息</param>
        /// <returns>更新后的审计信息Model</returns>
        AuditMessageModel AddMessage(AuditMessageModel model);

        /// <summary>
        /// 审计日志列表
        /// </summary>
        /// <returns>审计日志信息列表</returns>
        ListResult<AuditMessageModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);
    }
}
