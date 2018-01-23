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
    public partial interface IAcceptProxyService
    {
        /// <summary>
        /// 新增签收单
        /// </summary>
        /// <param name="acceptProxy"></param>
        /// <param name="isProduction">消息推送IOS环境,是否是生成环境</param>
        /// <returns></returns>
        AcceptProxyModel AddAcceptProxy(AcceptProxyModel acceptProxy, bool isProduction = false);

        /// <summary>
        /// 更新签收单
        /// </summary>
        /// <returns>是否更新成功</returns>
        bool UpdateAcceptProxy(UpdateAcceptProxyModel acceptProxy, int acceptProxyId);

        /// <summary>
        /// 删除签收单
        /// </summary>
        /// <returns>是否删除成功</returns>
        bool deleteAcceptProxy(int acceptProxyId);

        /// <summary>
        /// 查询当前用户的签收记录
        /// </summary>
        /// <returns>签收记录信息</returns>
        IPagedList<AcceptProxyModel> MyAcceptProxyList(int userId, int pageIndex, int pageSize);
        
        /// <summary>
        /// 查询所有用户的签收记录
        /// </summary>
        /// <returns>签收记录信息</returns>
        IPagedList<AcceptProxyModel> AcceptProxyHistoryList(int pageIndex, int pageSize);
        /// <summary>
        /// 查询签收信息
        /// </summary>
        /// <returns>签收记录信息</returns>
        AcceptProxy GetAcceptProxyById(int id);

        /// <summary>
        /// 代签信息列表
        /// </summary>
        /// <returns>代签息列表</returns>
        ListResult<AcceptProxyModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);


    }
}
