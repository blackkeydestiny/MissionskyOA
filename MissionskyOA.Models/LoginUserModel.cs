using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 后台管理用户登录Model
    /// </summary>
    public class LoginUserModel
    {
        [Display(Name = "UserName")]
        [Description("用户名")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Description("密码")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Description("记住我")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }
    }
}
