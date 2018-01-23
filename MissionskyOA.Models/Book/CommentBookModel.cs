using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 评价图书
    /// </summary>
    public class CommentBookModel
    {
        /// <summary>
        /// 图书ID
        /// </summary>
        [Description("图书ID")]
        public int BookId { get; set; }

        /// <summary>
        /// 图书评论ID(用户于编辑评论)
        /// </summary>
        [Description("图书评论ID(用户于编辑评论)")]
        public int? CommentId { get; set; }

        /// <summary>
        /// 图书评价
        /// </summary>
        [Description("图书评价")]
        public string Comment { get; set; }

        /// <summary>
        /// 图书评分
        /// </summary>
        [Description("图书评分")]
        public int Rating { get; set; }
    }
}
