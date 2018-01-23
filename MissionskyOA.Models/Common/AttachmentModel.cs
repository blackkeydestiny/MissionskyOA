using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 附件
    /// </summary>
    public class AttachmentModel : BaseModel
    {

        /// <summary>
        /// 类型: 图书封面等
        /// </summary>
        [Description("类型: 图书封面等")]
        public string EntityType { get; set; }

        /// <summary>
        /// ID: 图书ID等
        /// </summary>
        [Description("ID: 图书ID等")]
        public int EntityId { get; set; }

        /// <summary>
        /// 附件名
        /// </summary>
        [Description("附件名")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Desc { get; set; }

        /// <summary>
        /// 附件内容
        /// </summary>
        [Description("附件内容")]
        public byte[] Content { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [Description("上传时间")]
        public DateTime CreatedTime { get; set; }
    }
}
