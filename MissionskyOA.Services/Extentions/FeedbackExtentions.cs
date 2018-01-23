using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Models;
using MissionskyOA.Models.Feedback;
using log4net;
using MissionskyOA.Data;

namespace MissionskyOA.Services.Extentions
{
    public static class FeedbackExtentions
    {

        /// <summary>
        /// Log instance.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(FeedbackExtentions));

        public static Feedback ToEntity(this NewFeedback feed)
        {
            //验证feed
            feed.Valid();

            var entity = new Feedback()
            {
              Desc = feed.Description,
              //PictureID=feed.PictureID,
              ProblemType=(int)feed.ProblemType,
                //Theme=feed.Theme,
              Id = feed.UserID,
              CreateTime = DateTime.Now
            };

            return entity;
        }

        public static FeedbackModel ToModel(this Feedback entity)
        {
            var model = new FeedbackModel()
            {
               Description = entity.Desc,
               //PictureID = (int)entity.PictureID,
               //ProblemType = (int)entity.ProblemType,
               //Theme = entity.Theme,
               UserID = (int)entity.Id,
               Datetime = DateTime.Now
            };
            

            return model;
        }

        /// <summary>
        /// 验证新的反馈意见是否有效
        /// </summary>
        /// <param name="feed">新的工作任务</param>
        public static void Valid(this NewFeedback feed)
        {


            if (feed == null)
            {
                Log.Error("意见无效");
                throw new InvalidOperationException("意见无效");
            }

            if (string.IsNullOrEmpty(feed.Description))
            {
                Log.Error("问题描述无效");
                throw new InvalidOperationException("问题描述无效");
            }

            if (feed.PictureID <= 0)
            {
                Log.Error("图片无效");
                throw new InvalidOperationException("图片无效");
            }

            if (string.IsNullOrEmpty(feed.Theme))
            {
                 Log.Error("主题无效");
                throw new InvalidOperationException("主题无效");
            }

            if (feed.UserID <= 0)
            {
                 Log.Error("意见反馈人无效");
                throw new InvalidOperationException("意见反馈人无效");
            }

            //if ((int)feed.ProblemType!=1 || (int)feed.ProblemType!=2)
            //{
            //    Log.Error("问题类型无效");
            //    throw new InvalidOperationException("问题类型无效");
            //}
        }
    }
}
