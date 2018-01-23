using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MissionskyOA.Models.Account
{
    /// <summary>
    /// 修改密码
    /// </summary>
    public class UpdatePasswordModel
    {
        [Description("旧密码")]
        public string Password { get; set; }

        [Description("新密码")]
        public string NewPassword { get; set; }
    }
}
