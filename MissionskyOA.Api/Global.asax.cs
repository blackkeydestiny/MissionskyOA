using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MissionskyOA.Core.Enum;
using MissionskyOA.Services;

namespace MissionskyOA.Api
{
    /// <summary>
    /// WebApiApplication
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Start
        /// </summary>
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("启动Log4NET。");

            Bootstrapper.Start();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string path = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data");

            (new ScheduledTaskService()).Start(); //开始定时任务
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            string error = string.Empty;
            error += "发生时间:" + System.DateTime.Now.ToString() + "<br>";
            error += "发生异常页: " + Request.Url.ToString() + "<br>";
            error += "异常信息: " + objErr.Message + "<br>";
            error += "错误源:" + objErr.Source;
            error += "堆栈信息:" + objErr.StackTrace;
            error += "--------------------------------------<br>";

            log.Error(error);
        }
    }
}
