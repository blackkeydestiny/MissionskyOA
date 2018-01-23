using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 头像
    /// </summary>
    public class AvatarModel
    {
        /// 注： 与 Member 的记录是一对一关系
        [Description("用户ID")]
        public int UserId { get; set; }

        [Description("文件名")]
        public string FileName { get; set; }

        [Description("文件内容")]
        public byte[] Content { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }
    }
}
