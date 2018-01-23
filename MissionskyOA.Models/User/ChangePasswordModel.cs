using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tutor.Models
{
    /// <summary>
    /// 重置密码模型
    /// </summary>
    public class ChangePasswordModel
    {
        [Description("会员ID")]
        public int MemberId { get; set; }

        [Description("新密码")]
        public string Password { get; set; }

        [Description("重复密码")]
        public string RepeatedPassword { get; set; }
    }
}
