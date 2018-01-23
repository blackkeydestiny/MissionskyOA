using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    public class SingleUserModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Description("QQID")]
        public string QQID { get; set; }

        /// <summary>
        /// 手机电话号码
        /// </summary>
        [Description("手机")]
        public string Phone { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        [Description("中文名")]
        public string ChineseName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Description("英文名")]
        public string EnglishName { get; set; }

    }
}
