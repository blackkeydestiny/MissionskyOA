using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Models;
using MissionskyOA.Resources;

namespace MissionskyOA.Api.Filter
{
    /// <summary>
    /// 请求过滤器
    /// </summary>
    public class RequestBodyFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用
        /// </summary>
        /// <param name="actionContext"></param>
        public async override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            //获取请求消息提数据
            Stream stream = await actionContext.Request.Content.ReadAsStreamAsync();
            Encoding encoding = Encoding.UTF8;
            stream.Position = 0;
            string responseData = "";
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                responseData = reader.ReadToEnd().ToString();
            }

            //请求Body为空返回错误
            if (String.IsNullOrWhiteSpace(responseData))
            {
                actionContext.Response = new HttpResponseMessage();
                actionContext.Response.StatusCode = HttpStatusCode.OK;
                ApiResponse result = new ApiResponse()
                {
                    StatusCode = 500,
                    Message = "system error."                  
                };

                actionContext.Response.Content = new StringContent(JsonConvert.SerializeObject(result));
            }
        }
    }
}