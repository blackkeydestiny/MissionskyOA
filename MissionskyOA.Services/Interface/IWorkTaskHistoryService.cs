using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作任务历史服务接口
    /// </summary>
    public interface IWorkTaskHistoryService
    {
        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        bool AddHistory(MissionskyOAEntities dbContext, WorkTaskHistoryModel model);

        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="model"></param>
        bool AddHistory(WorkTaskHistoryModel model);

        /// <summary>
        /// 获取指定ID工作任务操作记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务操作记录</returns>
        IList<WorkTaskHistoryModel> GetWorkTaskHistoryList(MissionskyOAEntities dbContext, int taskId);

        /// <summary>
        /// 获取指定ID工作任务操作记录
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务操作记录</returns>
        IList<WorkTaskHistoryModel> GetWorkTaskHistoryList(int taskId);
    }
}
