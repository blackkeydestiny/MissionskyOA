using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;
using MissionskyOA.Api.Handlers;
using MissionskyOA.Services;

namespace MissionskyOA.Api
{


    /*
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * **/


    /// <summary>
    /// API 配置
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 注册函数
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());
            config.MessageHandlers.Add(new ApiRequestHandler(new UserTokenService()));
        }
    }
}
