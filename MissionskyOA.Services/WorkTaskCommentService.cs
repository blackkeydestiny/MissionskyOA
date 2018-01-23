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
    /// 工作任务评论
    /// </summary>
    public class WorkTaskCommentService : ServiceBase, IWorkTaskCommentService
    {
        /// <summary>
        /// 添加工作任务评论
        /// </summary>
        /// <param name="user">评论人</param>
        /// <param name="taskId">工作任务</param>
        /// <param name="comment">评论</param>
        public bool AddComment(UserModel user, int taskId, string comment)
        {
            if (taskId <= 0 || string.IsNullOrEmpty(comment))
            {
                Log.Error("无效的评论。");
                throw new InvalidOperationException("无效的评论。 ");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = new WorkTaskComment()
                {
                    Comment = comment,
                    UserId = user.Id,
                    TaskId = taskId,
                    CreatedTime = DateTime.Now
                };

                dbContext.WorkTaskComments.Add(entity); //添加评论

                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 修改工作任务评论
        /// </summary>
        /// <param name="user">评论人</param>
        /// <param name="commentId">工作任务评论Id</param>
        /// <param name="comment">评论</param>
        public bool ModifyComment(UserModel user, int commentId, string comment)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.WorkTaskComments.FirstOrDefault(it => it.Id == commentId);

                if (entity == null || string.IsNullOrEmpty(comment))
                {
                    Log.Error("无效的评论。");
                    throw new InvalidOperationException("无效的评论。 ");
                }

                entity.Comment = comment; //修改评论
                
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 删除工作任务评论
        /// </summary>
        /// <param name="commentId">备注id</param>
        public bool DeleteComment(int commentId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.WorkTaskComments.FirstOrDefault(it => it.Id == commentId);

                dbContext.WorkTaskComments.Remove(entity);
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 获取指定ID工作任务评论
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务评论</returns>
        public IList<WorkTaskCommentModel> GetWorkTaskCommentList(MissionskyOAEntities dbContext, int taskId)
        {
            var comments = new List<WorkTaskCommentModel>(); //评论
            var entities =
                          dbContext.WorkTaskComments.Where(it => it.TaskId == taskId).OrderByDescending(it => it.CreatedTime).ToList();

            entities.ForEach(entity =>
            {
                var model = entity.ToModel();

                //操作用户
                var user = dbContext.Users.FirstOrDefault(it => it.Id == model.UserId);
                model.UserName = (user == null ? string.Empty : user.EnglishName);

                comments.Add(model); //评论
            });
            return comments;
        }

        /// <summary>
        /// 获取指定ID工作任务评论
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务评论</returns>
        public IList<WorkTaskCommentModel> GetWorkTaskCommentList(int taskId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                return GetWorkTaskCommentList(dbContext, taskId);
            }
        }
    }
}
