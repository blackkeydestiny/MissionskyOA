using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MissionskyOA.Models;

namespace MissionskyOA.Portal.Common
{
    public class CommonInstance
    {
        private static CommonInstance instance;

        private CommonInstance()
        {

        }

        public static CommonInstance GetInstance()
        {
            if (instance == null)
            {
                instance = new CommonInstance();
            }
            return instance;
        }

        public UserModel LoginUser
        {
            set
            {
                HttpContext.Current.Session[CmnConstants.LOGIN_USER_NAME] = value;
            }
            get
            {
                if (HttpContext.Current.Session[CmnConstants.LOGIN_USER_NAME] != null)
                {
                    return (UserModel)HttpContext.Current.Session[CmnConstants.LOGIN_USER_NAME];
                }
                return null;
            }
        }

        /// <summary>
        /// 设置记住我们
        /// </summary>
        /// <param name="model"></param>
        public void SetRemeberMe(LoginUserModel model)
        {
            HttpContext.Current.Session[CmnConstants.SESSION_KEY_USERNAME_REMEBER] = model.UserName;
            HttpContext.Current.Session[CmnConstants.SESSION_KEY_PWD_REMEBER] = model.Password;
            HttpContext.Current.Session[CmnConstants.SEEION_KEY_REMEBER_ME] = model.RememberMe;
        }

        /// <summary>
        /// 获取记住我们的用户名和密码
        /// </summary>
        /// <returns></returns>
        public LoginUserModel GetRemeberMe()
        {
            return new LoginUserModel()
            {
                UserName = HttpContext.Current.Session[CmnConstants.SESSION_KEY_USERNAME_REMEBER] != null
                ? HttpContext.Current.Session[CmnConstants.SESSION_KEY_USERNAME_REMEBER].ToString()
                : string.Empty,

                Password = HttpContext.Current.Session[CmnConstants.SESSION_KEY_PWD_REMEBER] != null
                ? HttpContext.Current.Session[CmnConstants.SESSION_KEY_PWD_REMEBER].ToString()
                : string.Empty,
                RememberMe = HttpContext.Current.Session[CmnConstants.SEEION_KEY_REMEBER_ME] != null
                ? (bool)HttpContext.Current.Session[CmnConstants.SEEION_KEY_REMEBER_ME]
                : false

            };
        }

        /// <summary>
        /// 清空记住我的信息
        /// </summary>
        public void ClearRemeberMe()
        {
            HttpContext.Current.Session[CmnConstants.SESSION_KEY_USERNAME_REMEBER] = String.Empty;
            HttpContext.Current.Session[CmnConstants.SESSION_KEY_PWD_REMEBER] = String.Empty;
            HttpContext.Current.Session[CmnConstants.SEEION_KEY_REMEBER_ME] = false;
        }


    }
}