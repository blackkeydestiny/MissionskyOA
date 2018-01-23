using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using MissionskyOA.Core.Common;
using MissionskyOA.Services;
using MissionskyOA.Api;
using MissionskyOA.Models;
using MissionskyOA.Resources;
using System.Configuration;

namespace MissionskyOA.Api.Handlers
{
    /// <summary>
    /// API 请求处理器
    /// 
    /// 资料学习：
    /// https://www.cnblogs.com/artech/p/asp-net-web-api-pipeline.html
    /// </summary>
    public class ApiRequestHandler : DelegatingHandler
    {

        private IUserTokenService UserTokenService { get; set; }

        //
        private static readonly object Locker = new object();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static ApiRequestHandler()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memberTokenService"></param>
        public ApiRequestHandler(IUserTokenService userTokenService)
        {
            this.UserTokenService = userTokenService;
        }

        /// <summary>
        /// 检查用户登录状态，并以异步的方式请请求传送给下一个Http Request Handler
        /// </summary>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                return Task.Run<HttpResponseMessage>(() =>
                {
                    var resp = new HttpResponseMessage();
                    resp.Content = new StringContent("");
                    //resp.Content.Headers.Add("Access-Control-Allow-Origin", GetHeaderByKey(request, "Origin"));
                    resp.Content.Headers.Add("Access-Control-Allow-Origin", "*");
                    resp.Content.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE");
                    resp.Content.Headers.Add("Access-Control-Allow-Headers", "token,Content-Type,X-Requested-With");
                    return resp;
                });
            }
            
            try
            {
                lock (Locker)
                {
                    // check token except  register & login & resetpassword
                    if (request.RequestUri != null && IsNeededTokenApis(request.RequestUri.ToString()))
                    {
                        // 获取request中的token
                        var token = GetHeaderByKey(request, "token");
                        ApiWorkContext.Instance().Token = token;

                        if (string.IsNullOrEmpty(token) || !this.UserTokenService.IsVaild(token))
                        {
                            throw new InvalidCastException("Invalid token");
                        }
                        else
                        {
                            ApiWorkContext.Instance().User = UserTokenService.GetMemberByToken(token);
                        }
                    }
               } 

            }
            catch (InvalidCastException ex)
            {
                return Task.Run<HttpResponseMessage>(() =>
                {
                    return GenerateResponse(request, HttpStatusCode.Unauthorized, "Invalid token", ex.Message);
                });
            }

            var result = base.SendAsync(request, cancellationToken);

            //if (result.Result != null)
            //{
            //    if (result.Result.StatusCode == HttpStatusCode.NotFound)
            //    {
            //        return Task.Run<HttpResponseMessage>(() =>
            //           {
            //               return GenerateResponse(request, HttpStatusCode.NotFound, "The requested resource is not found", null);
            //           });
            //    }
            //}

            return result;
        }

        /// <summary>
        /// 从 HttpHeader 获取对应key 的 值
        /// </summary>
        /// <param name="request">Request 对象</param>
        /// <param name="key">Http Header 的 Key</param>
        /// <returns>值</returns>
        private static string GetHeaderByKey(HttpRequestMessage request, string key)
        {
            string value = null;

            try
            {
                value = request.Headers.GetValues(key).First();
            }
            catch { }

            return value;
        }

        /// <summary>
        /// 生成对应 Request 的 Response
        /// </summary>
        /// <param name="request">Request 对象</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">消息</param>
        /// <param name="detail">具体消息</param>
        /// <returns>Response 对象 </returns>
        private HttpResponseMessage GenerateResponse(HttpRequestMessage request, HttpStatusCode statusCode, string message, string detail)
        {
            var response = request.CreateResponse<ApiResponse>(statusCode, new ApiResponse
            {
                StatusCode = (int)statusCode,
                Message = message,
                MessageDetail = detail,
            });

            response.Content.Headers.Add("Access-Control-Allow-Origin", GetHeaderByKey(request, "Origin"));

            return response;
        }

        /// <summary>
        /// 是否是需要Token验证的API
        /// </summary>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        private bool IsNeededTokenApis(string apiUri)
        {
            bool bol = true;
            string[] unNeededTokenApis =
            {
                "api/accounts/register",
                "api/accounts/login",
                "api/accounts/resetpassword",
                "api/avatars/download",
                "api/acceptproxy/download",
                "api/books/download/cover",
                "api/attachment/download",
                "download",
                "upload",
                "test"
                //"api/orders"
            };

            for (int i = 0; i < unNeededTokenApis.Length; i++)
            {
                if (apiUri.LastIndexOf(unNeededTokenApis[i]) != -1)
                {
                    bol = false;
                    break;
                }
            }
            return bol;
        }

    }
}