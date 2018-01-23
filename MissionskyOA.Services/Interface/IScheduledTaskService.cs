using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 定时任务处理
    /// </summary>
    public partial interface IScheduledTaskService
    {
        /// <summary>
        /// 监听线程
        /// </summary>
        /// <returns>监听结果</returns>
        NameValueCollection Monitor();

        /// <summary>
        /// 启用禁用定时任务
        /// </summary>
        /// <param name="id">定时任务Id</param>
        void Enable(int id);

        /// <summary>
        /// 启动任务(默认启动所有任务)
        /// </summary>
        /// <param name="tasks">定时任务I</param>
        void Start(IList<ScheduledTaskModel> tasks = null);

        /// <summary>
        /// 手动执行定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns></returns>
        bool Execute(ScheduledTaskModel task);

        /// <summary>
        /// 获取所有定时任务
        /// </summary>
        /// <returns>定时任务列表</returns>
        IList<ScheduledTaskModel> GetScheduledTasks();

        /// <summary>
        /// 获取所有定时任务Id
        /// </summary>
        /// <returns>定时任务Id列表</returns>
        IList<int> GetScheduledTaskIds();

        /// <summary>
        /// 根据定时任务Id获取定时任务详细
        /// </summary>
        /// <param name="taskId">定时任务Id</param>
        /// <returns>定时任务详细</returns>
        ScheduledTaskModel GetScheduledTaskDetail(int taskId);

        /// <summary>
        /// 更新定时任务
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="taskId">定时任务Id</param>
        /// <param name="content">定时任务执行内容</param>
        /// <param name="executeDate">执行时间</param>
        /// <param name="isSavedChanges">是否存储数据库变化，默认不保存</param>
        /// <returns>true or false</returns>
        void UpdateTask(MissionskyOAEntities dbContext, int taskId, DateTime executeDate, string content, bool isSavedChanges = false);

        /// <summary>
        /// 查询实时任务
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <returns>实时任务列表</returns>
        ListResult<ScheduledTaskModel> TaskList(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 查询定时任务执行记录
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <param name="taskId">定时任务Id</param>
        /// <returns>实时定时任务执行记录</returns>
        ListResult<ScheduledTaskHistoryModel> TaskHistoryList(int pageNo, int pageSize, SortModel sort, FilterModel filter, int taskId);

        /// <summary>
        /// 添加定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns>task id</returns>
        int Add(ScheduledTaskModel task);

        /// <summary>
        /// 更新定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns>true or exception</returns>
        bool Update(ScheduledTaskModel task);

        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="id">定时任务id</param>
        /// <returns>是否删除成功</returns>
        bool Delete(int id);
    }
}
