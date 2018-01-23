using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MissionskyOA.Models.Account
{
    /// <summary>
    /// 重置密码
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// 电话号码
        /// </summary>
        [Description("电话号码")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        public string Email { get; set; }
    }
}
