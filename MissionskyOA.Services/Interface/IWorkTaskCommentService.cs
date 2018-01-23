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
    /// 工作任务评论服务接口
    /// </summary>
    public interface IWorkTaskCommentService
    {
        /// <summary>
        /// 添加工作任务评论
        /// </summary>
        /// <param name="user">评论人</param>
        /// <param name="taskId">工作任务</param>
        /// <param name="comment">评论</param>
        bool AddComment(UserModel user, int taskId, string comment);

        /// <summary>
        /// 修改工作任务评论
        /// </summary>
        /// <param name="user">评论人</param>
        /// <param name="commentId">工作任务评论Id</param>
        /// <param name="comment">评论</param>
        bool ModifyComment(UserModel user, int commentId, string comment);

        /// <summary>
        /// 删除工作任务评论
        /// </summary>
        /// <param name="commentId">备注id</param>
        bool DeleteComment(int commentId);

        /// <summary>
        /// 获取指定ID工作任务评论
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务评论</returns>
        IList<WorkTaskCommentModel> GetWorkTaskCommentList(MissionskyOAEntities dbContext, int taskId);

        /// <summary>
        /// 获取指定ID工作任务评论
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>工作任务评论</returns>
        IList<WorkTaskCommentModel> GetWorkTaskCommentList(int taskId);
    }
}
