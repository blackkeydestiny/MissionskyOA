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
    public partial interface IOvertimeService
    {
        /// <summary>
        /// 显示所有加班单
        /// </summary>
        /// <returns>所有加班单分页信息</returns>
        IPagedList<OrderModel> MyOvertimeList(UserModel model, int pageIndex, int pageSize);

        /// <summary>
        /// 显示加班单具体信息根据具体加班单信息id
        /// </summary>
        /// <returns>具体加班信息</returns>
        OrderDetModel GetOvertimeDetailsByID(int orderDetailID);

        /// <summary>
        /// 根据userid,显示所有加班历史记录
        /// </summary>
        /// <returns>加班单分页信息</returns>
        IPagedList<OrderModel> GetOvertimeHistoryByUserID(UserModel model, int pageIndex, int pageSize);
        
        /// <summary>
        /// 显示所有加班记录
        /// </summary>
        /// <returns>加班单信息</returns>
        ListResult<OrderModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 根据加班单id显示所有加班单
        /// </summary>
        /// <returns>具体加班信息</returns>
        ListResult<OrderDetModel> GetOvertimeDetailsByOrderID(int orderID);

        
    }
}
