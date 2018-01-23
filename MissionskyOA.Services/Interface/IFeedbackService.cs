using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Models.Feedback;
using MissionskyOA.Core.Pager;


namespace MissionskyOA.Services.Interface
{
    /// <summary>
    /// 意见反馈接口
    /// </summary>
    public interface IFeedbackService
    {
        /// <summary>
        /// 创建新意见反馈
        /// </summary>
        /// <param name="task"></param>
        /// <param name="currentUser"></param>
        /// <returns>返回新的feed id</returns>
        int Add(NewFeedback feed);

        /// <summary>
        /// 更新意见反馈
        /// </summary>
        /// <returns>是否更新成功</returns>
        bool UpdateFeedback(NewFeedback feed,int feedbackID);

        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <returns>是否删除成功</returns>
        bool DeleteFeedback(int feedbackID);

        /// <summary>
        /// 查询当前用户
        /// </summary>
        /// <returns>意见反馈</returns>
        ///IPagedList<FeedbackModel> MyFeedbackList(int userId, int pageIndex, int pageSize);

        /// <summary>
        /// 查询所有用户意见反馈
        /// </summary>
        /// <returns>意见反馈</returns>
        IPagedList<FeedbackModel> FeedbackHistoryList(int pageIndex, int pageSize);
    }
}
