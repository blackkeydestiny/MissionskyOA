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
    /// 图书
    /// </summary>
    public class BookModel : BaseModel
    {
        /// <summary>
        /// 书名
        /// </summary>
        [Description("书名")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Description("编码")]
        public string ISBN { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        [Description("二维码")]
        public string BarCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public string Type { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Description("简介")]
        public string Desc { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Description("作者")]
        public string Author { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Description("来源")]
        public BookSource Source { get; set; }

        /// <summary>
        /// 捐赠用户
        /// </summary>
        [Description("捐赠用户")]
        public int? Donor { get; set; }

        /// <summary>
        /// 捐赠用户姓名
        /// </summary>
        [Description("捐赠用户姓名")]
        public string DonorName { get; set; }

        /// <summary>
        /// 当前借阅用户
        /// </summary>
        [Description("当前借阅用户")]
        public int? Reader { get; set; }

        /// <summary>
        /// 状态: 在库，借出, 遗失
        /// </summary>
        [Description("状态: 在库，借出, 遗失")]
        public BookStatus Status { get; set; }
        
        /// <summary>
        /// 入库日期
        /// </summary>
        [Description("入库日期")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 当前登录用户借阅信息
        /// </summary>
        [Description("当前登录用户借阅信息")]
        public BookBorrowModel CurrentUserBorrow { get; set; }

        /// <summary>
        /// 图书当前借阅信息
        /// </summary>
        [Description("图书当前借阅信息")]
        public BookBorrowModel BorrowBook { get; set; }

        /// <summary>
        /// 图书评分
        /// </summary>
        [Description("图书评分")]
        public BookRatingModel Rating { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Description("评价")]
        public IList<BookCommentModel> BookComments { get; set; }

        /// <summary>
        /// 图书评价个数
        /// </summary>
        [Description("图书评价个数")]
        public int BookCommentCount { get; set; }

        /// <summary>
        /// 图书封面
        /// </summary>
        [Description("图书封面")]
        public IList<int> BookCovers { get; set; }
    }
}
