using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models.Feedback;
using MissionskyOA.Services;
using MissionskyOA.Services.Extentions;
using MissionskyOA.Services.Interface;
using MissionskyOA.Models;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 意见反馈
    /// </summary>
    [RoutePrefix("api/feedback")]
    public class FeedbackController : BaseController
    {

        private IFeedbackService FeedbackServices { get; set; }

         /// <summary>
        /// 构造函数
        /// </summary>
        public FeedbackController(IFeedbackService feedbackServices)
        {
            this.FeedbackServices = feedbackServices;

        }

        /// <summary>
        /// 创建意见反馈
        /// </summary>
        /// <param name="feed">意见反馈详细</param>
        /// <returns>返回新的feed id</returns>
        [Route("add")]
        [HttpPost]
        public ApiResponse<int> AddWorkTask(NewFeedback feed)
        {
            feed.Valid(); //验证feed参数是否有效

            var response = new ApiResponse<int>()
            {
                Result = FeedbackServices.Add(feed)
            };

            return response;
        }

        /// <summary>
        /// 更新意见反馈
        /// </summary>
        /// <returns>是否更新成功</returns>
        [Route("update/{id}")]
        [HttpPut]
        public ApiResponse<bool> UpdateFeedback(int id, NewFeedback model)
        {
            if (id < 0 || model == null)
            {
                throw new ApiBadRequestException("Bad request");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = FeedbackServices.UpdateFeedback(model, id)
            };
            return response;
        }

        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <returns>是否删除成功</returns>
        [Route("Remove/{id}")]
        [HttpGet]
        public ApiResponse<bool> DeleteFeedback(int id)
        {
            if (id < 0)
            {
                throw new ApiBadRequestException("Bad request");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = FeedbackServices.DeleteFeedback(id)
            };
            return response;
        }

        /// <summary>
        /// 查询所有用户意见反馈
        /// </summary>
        /// <returns>意见反馈</returns>
        [Route("history/list")]
        [HttpGet]
        public ApiPagingListResponse<FeedbackModel> FeedbackHistoryList(int pageIndex = 0, int pageSize = 15)
        {
            // 3. 获取所有意见反馈
            var query = this.FeedbackServices.FeedbackHistoryList(pageIndex, pageSize);

            var result = new PaginationModel<FeedbackModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<FeedbackModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }
	}
}