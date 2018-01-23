using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MissionskyOA.Core.Email
{
    /// <summary>
    /// 获取邮件服务器配置
    /// </summary>
    public class EmailConfig
    {
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        /// 发送邮箱
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string Password { get; private set; }

        public EmailConfig()
        {
            Server = ConfigurationManager.AppSettings["EmailServer"];
            Email = ConfigurationManager.AppSettings["EmailAccount"];
            Password = ConfigurationManager.AppSettings["EmailPassword"];
        }
    }
}
