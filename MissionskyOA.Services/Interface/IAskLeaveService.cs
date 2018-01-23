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
    public partial interface IAskLeaveService
    {

        /// <summary>
        /// 显示所有请假单
        /// </summary>
        /// <returns>请假单分页信息</returns>
       IPagedList<OrderModel> MyAskLeaveList(UserModel model, int pageIndex, int pageSize);

        /// <summary>
        /// 显示具体请假信息
        /// </summary>
        /// <returns>具体请假信息</returns>
        OrderDetModel getAskLeaveDetailsByID(int orderDetailID);

        /// <summary>
        /// 根据userid,显示请假历史记录
        /// </summary>
        /// <returns>请假单分页信息</returns>
        IPagedList<OrderModel> GetAskLeaveHistoryByUserID(UserModel model, int pageIndex, int pageSize);
        
        /// <summary>
        /// 添加请假信息
        /// </summary>
        /// <param name="model">申请单详细</param>
        /// <param name="applicant">流程申请人</param>
        /// <returns>请假单信息</returns>
        OrderModel AddOrder(ApplyOrderModel model, UserModel applicant);

         /// <summary>
        /// 修改请假信息
        /// </summary>
        /// <returns>请假单信息</returns>
        bool UpdateOrder(int userId, int orderNo, UpdateOrderModel model);

        /// <summary>
        /// 销请假单或加班单
        /// </summary>
        /// <returns>取消</returns>
        bool RevokeOrder(int orderNo, RevokeOrderModel model, UserModel applicant);

        /// <summary>
        /// 取消请假单或者加班单
        /// </summary>
        /// <returns>取消</returns>
        bool CancelOrder(int orderNo, UserModel model);

        /// <summary>
        /// 显示所有请假记录
        /// </summary>
        /// <returns>请假单信息</returns>
        ListResult<OrderModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 根据请假单id显示所有请假单详细
        /// </summary>
        /// <returns>具体加班信息</returns>
        ListResult<OrderDetModel> GetAskLeaveDetailsByOrderID(int orderID);

        /// <summary>
        /// 根据id显示所有单据
        /// </summary>
        /// <returns>具体单据信息</returns>
        OrderModel GetOrderDetail(int id);

        /// <summary>
        /// 获取申请单详细信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="id">申请单Id</param>
        /// <param name="hasDetail"></param>
        /// <returns></returns>
        OrderModel GetOrderDetail(MissionskyOAEntities dbContext, int id, bool hasDetail = true);

        /// <summary>
        /// 根据申请单号OrderNo获取申请单详细信细
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        OrderModel GetOrderDetailByOrderNo(int orderNo);

        /// <summary>
        /// 是否时间段重复申请
        /// </summary>
        /// <returns>是否时间段重复申请</returns>
        string IsOrderTimeAvailiable(ApplyOrderModel model, int updateOrderId);

        /// <summary>
        /// 是否撤销的时间的在当前请假时间内
        /// </summary>
        /// <returns>是否撤销的时间的在当前请假时间内</returns>
        bool IsRevokeTimeAvailiable(int orderNo, RevokeOrderModel model);
    }
}
