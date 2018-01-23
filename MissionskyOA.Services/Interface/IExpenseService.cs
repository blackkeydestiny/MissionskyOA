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
    public partial interface IExpenseService
    {
        /// <summary>
        /// 添加报销申请单
        /// </summary>
        /// <param name="model">报销单</param>
        /// <returns>报销申请</returns>
        ExpenseMainModel AddExpense(ExpenseMainModel model);

        /// <summary>
        /// 显示所有报销单
        /// </summary>
        /// <returns>报销单分页信息</returns>
        IPagedList<ExpenseMainModel> MyExpenseList(UserModel model, int pageIndex, int pageSize);

        /// <summary>
        /// 显示具体报销单
        /// </summary>
        /// <returns>显示具体报销单</returns>
        List<ExpenseDetailModel> getExpenseDetailByID(int id);

        /// <summary>
        /// 显示所有报销单
        /// </summary>
        /// <returns>报销单</returns>
        ListResult<ExpenseMainModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);


        /// <summary>
        /// 获取用户待处理的报销单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>审批任务列表</returns>
        List<ExpenseMainModel> getPendingAuditLists(int userId);

        /// <summary>
        /// 批准或者拒绝报销单
        /// </summary>
        /// <param name="expenseId">申请单号</param>
        /// <param name="reason">理由</param>
        /// <param name="approver">审批人</param>
        /// <param name="isApprove">是否同意报销申请</param>
        /// <returns>是否审批成功</returns>
        bool ApproveOrRejectExpense(int expenseId, String reason, UserModel approver,bool isApprove);

        /// <summary>
        /// 发送报销单
        /// </summary>
        /// <param name="expenseId"></param>
        /// <returns></returns>
        bool SendExpenseForm(int expenseId);

        /// <summary>
        /// 统计审批人审批(待审批和已审批)报销单
        /// </summary>
        /// <param name="userId">审批人</param>
        /// <returns>审批人审批报销单</returns>
        List<ExpenseAuditSummaryModel> ExpenseSummary(int userId);
        
        /// <summary>
        /// 取消报销单
        /// </summary>
        /// <returns>是否成功</returns>
        bool CancelExpense(int expenseId, UserModel user);

         /// <summary>
        /// 修改报销信息
        /// </summary>
        /// <returns>是否成功</returns>

         bool UpdateExpenseOrder(int expenseId, UpdateExpenseModel model,UserModel user);

        /// <summary>
        /// 确认接收资料
        /// </summary>
        /// <param name="expenseId">申请单号</param>
        /// <param name="approver">审批人</param>
        /// <returns>是否成功</returns>
         bool ReciveExpenseFile(int expenseId, UserModel approver);

        
        /// <summary>
        /// 修改报销详细信息
        /// </summary>
        /// <returns>是否成功</returns>
         bool UpdateExpenseDetailsOrder(int id, ApplyExpenseDetailModel model,UserModel user);

         /// <summary>
         /// 删除报销单详细信息
         /// </summary>
         /// <returns>是否成功</returns>
         bool DeleteExpenseDetailsOrder(int id);

         /// <summary>
         /// 添加报销详细信息
         /// </summary>
         /// <returns>是否成功</returns>
         bool AddExpenseDetailsOrder(int id, ApplyExpenseDetailModel model);

    }
}
