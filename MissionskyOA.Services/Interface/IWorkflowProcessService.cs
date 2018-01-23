using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作流处理接口
    /// </summary>
    public interface IWorkflowProcessService
    {
        /// <summary>
        /// 添加流程处理记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="currentUser"></param>
        /// <param name="operation"></param>
        void AddWorkflowProcess(MissionskyOAEntities dbContext, UserModel currentUser, OperateOrderModel operation);

        /// <summary>
        /// 流程处理
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <param name="operation">流程操作</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns>流程处理对象</returns>
        WorkflowProcessModel Process(MissionskyOAEntities dbContext, OrderModel order, UserModel currentUser, OperateOrderModel operation);
    }
}
