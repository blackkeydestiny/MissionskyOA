using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 登录模型
    /// </summary>
    public class LoginModel : BaseModel
    {
        [Description("邮箱")]
        public string Email { get; set; }

        [Description("密码")]
        public string Password { get; set; }

        [Description("手机")]
        public string Phone { get; set; }

        [Description("QQID")]
        public string QQID { get; set; }
    }
}
