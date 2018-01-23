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
    /// 图书评分
    /// </summary>
    public class BookRatingModel
    {
        /// <summary>
        /// 评论数
        /// </summary>
        [Description("评论数")]
        public int Count { get; set; }

        /// <summary>
        /// 总评分
        /// </summary>
        [Description("总评分")]
        public int TotalRating { get; set; }

        /// <summary>
        /// 平均评分
        /// </summary>
        [Description("平均评分")]
        public int AverageRating { get; set; }
    }
}
