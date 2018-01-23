using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using log4net;
using MissionskyOA.Models;


namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    public abstract class BaseController : ApiController
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseController));

        /// <summary>
        /// 登录用户信息
        /// </summary>
        public UserModel Member { get; private set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; private set; }

        public BaseController()
        {
            this.Token = ApiWorkContext.Instance().Token;
            this.Member = ApiWorkContext.Instance().User;
        }
    }
}