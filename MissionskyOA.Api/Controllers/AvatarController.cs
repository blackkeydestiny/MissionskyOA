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

namespace MissionskyOA.Api.Controllers
{
    [RoutePrefix("api/avatars")]
    public class AvatarController : BaseController
    {
        private IAvatarService AvatarService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AvatarController(IAvatarService avatarService)
        {
            this.AvatarService = avatarService;
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns>头像链接</returns>
        [Route("upload")]
        [HttpPost]
        public ApiResponse<string> Upload()
        {
            AvatarModel model = new AvatarModel();
            string fileName = string.Format("{0}.png", Guid.NewGuid().ToString("N"));
            model.FileName = fileName;
            model.UserId = Member.Id;

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            Stream fileStream = file.InputStream;
            byte[] buffer = new byte[file.ContentLength];
            fileStream.Read(buffer, 0, file.ContentLength);
            model.Content = buffer;
            model.CreatedTime = DateTime.Now;
            this.AvatarService.Save(model);

            ApiResponse<string> response = new ApiResponse<string>()
            {
                Result = "api/avatars/download/" + this.Member.Id
            };

            return response;
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <returns>头像文件流</returns>
        [Route("download/{userId:int}")]
        [HttpGet]
        public HttpResponseMessage Download(int userId)
        {
            var model = this.AvatarService.GetByUserId(userId);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new ByteArrayContent(model.Content);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return result;
        }

    }
}
