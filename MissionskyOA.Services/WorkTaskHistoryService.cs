using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Services.Extentions;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作任务历史
    /// </summary>
    public class WorkTaskHistoryService : ServiceBase, IWorkTaskHistoryService
    {
        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        public bool AddHistory(MissionskyOAEntities dbContext, WorkTaskHistoryModel model)
        {
            if (model == null)
            {
                Log.Error("无效的操作历史记录。");
                throw new InvalidOperationException("无效的操作历史记录。 ");
            }

            dbContext.WorkTaskHistories.Add(model.ToEntity());

            return true;
        }
        
        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="model"></param>
        public bool AddHistory(WorkTaskHistoryModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                AddHistory(dbContext, model);

                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 获取指定ID工作任务操作记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务操作记录</returns>
        public IList<WorkTaskHistoryModel> GetWorkTaskHistoryList(MissionskyOAEntities dbContext, int taskId)
        {
            var histories = new List<WorkTaskHistoryModel>(); //操作记录
            var entities =
                          dbContext.WorkTaskHistories.Where(it => it.TaskId == taskId).OrderByDescending(it => it.CreatedTime).ToList();

            entities.ForEach(entity =>
            {
                var model = entity.ToModel();

                //操作用户
                var user = dbContext.Users.FirstOrDefault(it => it.Id == model.Operator);
                model.OperatorName = (user == null ? string.Empty : user.EnglishName);

                histories.Add(model); //操作记录
            });
            return histories;
        }

        /// <summary>
        /// 获取指定ID工作任务操作记录
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务操作记录</returns>
        public IList<WorkTaskHistoryModel> GetWorkTaskHistoryList(int taskId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                return GetWorkTaskHistoryList(dbContext, taskId);
            }
        }
    }
}
