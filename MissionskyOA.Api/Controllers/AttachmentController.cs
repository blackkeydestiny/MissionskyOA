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
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 附件
    /// </summary>
    [RoutePrefix("api/attachment")]
    public class AttachmentController : BaseController
    {
        private IAttachmentService AttachmentService { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public AttachmentController(IAttachmentService attachmentService)
        {
            this.AttachmentService = attachmentService;
        }

        /// <summary>
        /// 根据附件Id下载附件
        /// </summary>
        /// <param name="id">附件Id</param>
        /// <returns>附件文件流</returns>
        [Route("download/{id}")]
        [HttpGet]
        public HttpResponseMessage Download(int id)
        {
            var model = this.AttachmentService.GetAttathcmentDetail(id);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            
            if (model != null)
            {
                result.Content = new ByteArrayContent(model.Content);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            }
            else
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }
        
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="entityId">附件信息</param>
        /// <param name="entityType">附件信息</param>
        /// <returns>上传成功或失败</returns>
        public static ApiResponse<bool> Upload(int entityId, string entityType)
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                Stream fileStream = file.InputStream;
                var buffer = new byte[file.ContentLength];
                var fileName = string.Format("{0}", Guid.NewGuid().ToString("N"));
                fileStream.Read(buffer, 0, file.ContentLength);

                var attachment = new AttachmentModel()
                {
                    Content = buffer,
                    EntityId = entityId,
                    EntityType = entityType,
                    Desc = file.FileName,
                    Name = fileName,
                    CreatedTime = DateTime.Now
                };

                var response = new ApiResponse<bool>()
                {
                    Result = (new AttachmentService()).Upload(attachment)
                };

                return response;
            }

            return new ApiResponse<bool>() { Result = false };
        }
    }
}
