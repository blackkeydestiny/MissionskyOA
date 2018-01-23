using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Services.Interface;
using MissionskyOA.Models.Feedback;
using MissionskyOA.Data;
using MissionskyOA.Core;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Models;
using MissionskyOA.Services.Extentions;

namespace MissionskyOA.Services
{
    public class FeedbackService : ServiceBase, IFeedbackService
    {
        /// <summary>
        /// 创建意见反馈
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public int Add(NewFeedback feed)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //创建意见反馈
                var entity = feed.ToEntity();
                dbContext.Feedbacks.Add(entity);
                dbContext.SaveChanges();
                return entity.Id;
            }
        }

        /// <summary>
        /// 更新意见反馈
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool UpdateFeedback(NewFeedback feed, int feedbackID)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var NewFeedbackEntity = dbContext.Feedbacks.Where(it => it.Id == feedbackID).FirstOrDefault();
                if (NewFeedbackEntity == null)
                {
                    throw new Exception("This express doesn't exist.");
                }

                NewFeedbackEntity.Desc = feed.Description;
                NewFeedbackEntity.Id = feed.PictureID;
                NewFeedbackEntity.ProblemType = (int)feed.ProblemType;
                //NewFeedbackEntity.Theme = feed.Theme;
                NewFeedbackEntity.UserId = feed.UserID;
                NewFeedbackEntity.CreateTime = feed.Datetime; 
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <returns>是否删除成功</returns>
        public bool DeleteFeedback(int feedbackID)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var NewFeedbackEntity = dbContext.Feedbacks.Where(it => it.Id == feedbackID).FirstOrDefault();
                if (NewFeedbackEntity == null)
                {
                    throw new Exception("This express doesn't exist.");
                }
                dbContext.Feedbacks.Remove(NewFeedbackEntity);
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 查询所有用户意见反馈
        /// </summary>
        /// <returns>意见反馈</returns>
        public IPagedList<FeedbackModel> FeedbackHistoryList(int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<FeedbackModel> result = new List<FeedbackModel>();

                var Feedbacklist = dbContext.Feedbacks.OrderByDescending(item => item.CreateTime);

                if (Feedbacklist == null)
                {
                    return null;

                }
                Feedbacklist.ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return new PagedList<FeedbackModel>(result, pageIndex, pageSize);
            }
        }
    }
}
