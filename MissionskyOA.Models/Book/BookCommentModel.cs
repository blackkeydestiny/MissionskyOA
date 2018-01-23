using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 图书评价
    /// </summary>
    public class BookCommentModel : BaseModel
    {
        /// <summary>
        /// 评价用户
        /// </summary>
        [Description("评价用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 评价用户姓名
        /// </summary>
        [Description("评价用户姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 图书ID
        /// </summary>
        [Description("图书ID")]
        public int BookId { get; set; }

        /// <summary>
        /// 评价详细
        /// </summary>
        [Description("评价详细")]
        public string Comment { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [Description("评分")]
        public int Rating { get; set; }

        /// <summary>
        /// 评价日期
        /// </summary>
        [Description("评价日期")]
        public DateTime CreatedDate { get; set; }
    }
}
