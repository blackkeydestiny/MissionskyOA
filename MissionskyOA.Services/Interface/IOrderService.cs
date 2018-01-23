using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 申请单处理
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// 获取当前申请单号
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns>申请单号</returns>
        int GenerateOrderNo(MissionskyOAEntities dbContext);

        /// <summary>
        /// 根据申请单号和用户Id获取申请单Id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="orderNo">申请单号</param>
        /// <param name="userId">申请用户</param>
        /// <returns></returns>
        int GetOrderIdByOrderNoAndUserId(MissionskyOAEntities dbContext, int orderNo, int userId);

        /// <summary>
        /// 流程申请
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <returns>流程处理对象</returns>
        WorkflowProcessModel Apply(MissionskyOAEntities dbContext, OrderModel order);

        /// <summary>
        /// 批准请假加班申请(撤销)单
        /// </summary>
        /// <param name="model">申请单</param>
        /// <param name="approver">审批人</param>
        /// <returns>是否审批成功</returns>
        int Approve(OperateOrderModel model, UserModel approver);

        /// <summary>
        /// 拒绝请假加班申请(撤销)单
        /// </summary>
        /// <param name="model">申请单</param>
        /// <param name="approver">审批人</param>
        /// <returns>是否拒绝成功</returns>
        bool Rejecte(OperateOrderModel model, UserModel approver);

        /// <summary>
        /// 流程撤销
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <param name="owner">申请人</param>
        /// <param name="operation">流程操作</param>
        /// <returns>下一个审批步骤</returns>
        WorkflowProcessModel Revoke(MissionskyOAEntities dbContext, OrderModel order, UserModel owner,
            OperateOrderModel operation);

        /// <summary>
        /// 工作流处理消息推送
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="order">申请单</param>
        /// <param name="operaterId">消息推送人Id</param>
        /// <param name="process">流程处理</param>
        /// <param name="receiverId">消息接收人Id</param>
        /// <remarks>receiverId = 0: 表示默认发送到代申请人</remarks>
        void AddNotification(MissionskyOAEntities dbContext, OrderModel order, int operaterId, WorkflowProcessModel process, int receiverId = 0);

        /// <summary>
        /// 请假工作交接通知
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="order">申请单</param>
        void AddNotification(MissionskyOAEntities dbContext, OrderModel order);

        /// <summary>
        /// 获取用户待处理的申请单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">申请单状态</param>
        /// <returns>审批任务列表</returns>
        List<OrderModel> GetPendingOrdersByUserId(int userId, OrderStatus? status = null);

        /// <summary>
        /// 获取用户待处理的申请单数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PendingOrderCountModel CountPendingOrders(int userId);

        /// <summary>
        /// 统计审批人审批(待审批和已审批)申请单
        /// </summary>
        /// <param name="userId">审批人</param>
        /// <param name="status">申请单状态</param>
        /// <returns>审批人审批申请单</returns>
        List<ApproveOrderModel> SummaryApproveOrders(int userId, OrderStatus? status = null);

        /// <summary>
        /// 查询关联人员的申请单
        /// </summary>
        /// <param name="userId">审批人</param>
        /// <returns>审批人审批申请单</returns>
        List<ApproveOrderModel> SearchApproveOrders(string englishname, int userId);
    }
}
