using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Common;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 通知消息
    /// </summary>
    [RoutePrefix("api/notification")]
    public class NotificationController : BaseController
    {
        private INotificationService NotificationService { get; set; }
        private IUserService UserService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public NotificationController(INotificationService notificationService, IUserService userService)
        {
            this.NotificationService = notificationService;
            this.UserService = userService;
        }

        /// <summary>
        /// 获取我的通知列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("mylist")]
        [HttpGet]
        public ApiPagingListResponse<NotificationModel> GetMyNotifications(int pageIndex = 0, int pageSize = 25)
        {
            // 注：消息属于个人隐私信息，所以不宜公开
            var query = this.NotificationService.GetMyNotifications(this.Member.Id, pageIndex, pageSize);

            var result = new PaginationModel<NotificationModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount,
            };

            return new ApiPagingListResponse<NotificationModel>
            {
                Result = query.Result,
                Page = result.Page,
            };
        }

        /// <summary>
        /// 发送QQ消息给User
        /// </summary>
        /// <param name="userid">信息接受者user id</param>
        /// <param name="messagecontent">信息</param>
        /// <returns></returns>
        [Route("qqmessage/send")]
        [HttpPut]
        public ApiResponse<bool> SendQQMessage(int userid, string messagecontent)
        {
            var user = this.UserService.GetUserDetail(userid);
            if(user==null)
            {
                throw new Exception("用户不存在.");
            }
            if (user.QQID == null)
            {
                throw new Exception("用户QQ信息不存在.");
            }
            string url = Global.SmartQQHost + "/Send?" + "qqid=" + user.QQID + "&messgecontent=" + messagecontent;
            string dat = HttpUtil.Post(url, "", 50000);

            if (dat.Equals("exception"))
            {
                throw new Exception("发送失败.");
            }
            else
            {
                ApiResponse result = (ApiResponse)JsonConvert.DeserializeObject(dat, typeof(ApiResponse));
                if(result.StatusCode==200)
                {
                    return new ApiResponse<bool>
                    {
                        Result = true,
                        Message=result.Message
                    };
                }
                else
                {
                    return new ApiResponse<bool>
                    {
                        StatusCode=500,
                        Result = false,
                        Message=result.Message
                    };
                }
            }
            
        }
    }
}
