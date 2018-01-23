using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Enum;
using System.Configuration;


namespace MissionskyOA.Api.Controllers
{
    [RoutePrefix("api/acceptproxy")]
    public class AcceptProxyController : BaseController
    {
        private IAcceptProxyService AcceptProxyService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="overtimeService">加班管理服务</param>
        /// <param name="askLeaveService">请假管理服务</param>
        /// <param name="userService">用户管理服务</param>
        public AcceptProxyController(IAcceptProxyService AcceptProxyService)
        {
            this.AcceptProxyService = AcceptProxyService;
        }

        /// <summary>
        /// 新增签收单
        /// </summary>
        /// <returns>签收信息</returns>
        [Route("addProxy")]
        [HttpPost]
        public ApiResponse<AcceptProxyModel> AddAcceptProxy()
        {
            // 1. 检查输入参数
            string fileName = null;
            byte[] buffer = null;
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                Stream fileStream = file.InputStream;
                buffer = new byte[file.ContentLength];
                fileName = string.Format("{0}", Guid.NewGuid().ToString("N"));
                fileStream.Read(buffer, 0, file.ContentLength);
            }


            // 2. 构造签收信息
            var acceptProxy = new AcceptProxyModel()
            {
                Date = DateTime.Now,
                FileName = fileName,
                Content = buffer,
                CreateUserId = this.Member.Id,
                LastModifyUserId = this.Member.Id,
                LastModifyTime = DateTime.Now
            };
            foreach (var key in HttpContext.Current.Request.Form.AllKeys)
            {
                //接收FormData  
                switch (key)
                {
                    case "description":
                        acceptProxy.Description = HttpContext.Current.Request.Form[key];
                        break;
                    case "acceptUserId":
                        acceptProxy.AcceptUserId = int.Parse(HttpContext.Current.Request.Form[key]);
                        break;
                    case "status":
                        int currentStatus = int.Parse(HttpContext.Current.Request.Form[key]);
                        acceptProxy.Status = (ExpressStatus)currentStatus;
                        break;
                    case "courier":
                        acceptProxy.Courier = HttpContext.Current.Request.Form[key];
                        break;
                    case "leaveMessage":
                        acceptProxy.LeaveMessage = HttpContext.Current.Request.Form[key];
                        break;
                    case "courierPhone":
                        acceptProxy.CourierPhone = HttpContext.Current.Request.Form[key];
                        break;
                    default:
                        break;
                }

            }

            bool isProduction = false;
            if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
            {
                isProduction = true;
            }
            // 3. Construct API Response
            ApiResponse<AcceptProxyModel> response = new ApiResponse<AcceptProxyModel>()
            {
                Result = AcceptProxyService.AddAcceptProxy(acceptProxy, isProduction)
            };

            return response;
        }


        ///<summary>
        ///下载签收记录附件
        ///</summary>
        ///<returns>附件文件流</returns>
        [Route("download/{id}")]
        [HttpGet]
        public HttpResponseMessage Download(int id)
        {
            var model = this.AcceptProxyService.GetAcceptProxyById(id);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new ByteArrayContent(model.Content);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return result;
        }

        /// <summary>
        /// 更新签收单
        /// </summary>
        /// <returns>是否更新成功</returns>
        [Route("{id}/update")]
        [HttpPut]
        public ApiResponse<bool> UpdateAcceptProxy(int id, UpdateAcceptProxyModel model)
        {
            if (id < 0 || model == null)
            {
                throw new ApiBadRequestException("Bad request");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = AcceptProxyService.UpdateAcceptProxy(model, id)
            };
            return response;
        }

        /// <summary>
        /// 删除签收单
        /// </summary>
        /// <returns>是否删除成功</returns>
        [Route("{id}/Remove")]
        [HttpGet]
        public ApiResponse<bool> DeleteAcceptProxy(int id)
        {
            if (id < 0)
            {
                throw new ApiBadRequestException("Bad request");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = AcceptProxyService.deleteAcceptProxy(id)
            };
            return response;
        }

        /// <summary>
        /// 查询当前用户的代签和签收记录
        /// </summary>
        /// <returns>代签和签收记录信息</returns>
        [Route("get")]
        [HttpGet]
        public ApiPagingListResponse<AcceptProxyModel> MyAcceptProxyList(int pageIndex = 0, int pageSize = 15)
        {
            // 3. 获取所有请假单信息
            var query = this.AcceptProxyService.MyAcceptProxyList(this.Member.Id, pageIndex, pageSize);

            var result = new PaginationModel<AcceptProxyModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<AcceptProxyModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 查询所有用户的签收记录
        /// </summary>
        /// <returns>签收记录信息</returns>
        [Route("history/list")]
        [HttpGet]
        public ApiPagingListResponse<AcceptProxyModel> AcceptProxyHistoryList(int pageIndex = 0, int pageSize = 15)
        {
            // 3. 获取所有请假单信息
            var query = this.AcceptProxyService.AcceptProxyHistoryList(pageIndex, pageSize);

            var result = new PaginationModel<AcceptProxyModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<AcceptProxyModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }
    }
}
