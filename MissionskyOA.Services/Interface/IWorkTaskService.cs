using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core;
using MissionskyOA.Core.Pager;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作任务服务接口
    /// </summary>
    public interface IWorkTaskService
    {
        /// <summary>
        /// 创建新工作任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="currentUser"></param>
        /// <returns>返回新的Task id</returns>
        int Add(NewWorkTaskModel task, UserModel currentUser);

        /// <summary>
        /// 修改工作任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="task"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        bool Edit(int taskId, NewWorkTaskModel task, UserModel currentUser);

        /// <summary>
        /// 取消工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="season">取消工作任务原因</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        bool Cancel(int taskId, string season, UserModel currentUser);

        /// <summary>
        /// 转移(指派)工作任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="executor">执行人</param>
        /// <param name="currentUser"></param>
        /// <param name="comment">备注信息</param>
        /// <returns></returns>
        bool Transfer(int taskId, int executor, UserModel currentUser, string comment = null);

        /// <summary>
        /// 执行工作任务工作任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="nextStatus"></param>
        /// <param name="currentUser"></param>
        /// <param name="comment">备注信息</param>
        /// <returns></returns>
        bool Do(int taskId, WorkTaskStatus nextStatus, UserModel currentUser, string comment = null);

        /// <summary>
        /// 分页条件查询工作任务
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的工作任务</returns>
        IPagedList<WorkTaskModel> SearchByCriteria(SearchWorkTaskModel search, int pageIndex, int pageSize);
        
        /// <summary>
        /// 根据工作任务ID获取详细信息
        /// </summary>
        /// <param name="taskId">工作任务id</param>
        /// <returns>工作任务详细信息</returns>
        WorkTaskModel GetWorkTaskDetail(int taskId);

        /// <summary>
        /// 统计个人任务信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<WorkTaskSummaryModel> SummaryWorkTask(int userId);

        /// <summary>
        /// 分页获取工作任务信息
        /// </summary>
        /// <returns>获取会工作任务信息</returns>
        ListResult<WorkTaskModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);
    }
}
