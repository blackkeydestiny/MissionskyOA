using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 图书借阅处理接口
    /// </summary>
    public interface IBookCommentService
    {
        /// <summary>
        /// 汇总图书评分
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <returns>图书评分</returns>
        BookRatingModel CountBookRading(int bookId);

        /// <summary>
        /// 获取指定ID图书的评论
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="bookId">图书ID</param>
        /// <param name="rating">评论汇总(输出)</param>
        /// <returns>评论列表</returns>
        IList<BookCommentModel> GetBookCommentList(MissionskyOAEntities dbContext, int bookId, BookRatingModel rating = null);


        /// <summary>
        /// 获取指定ID图书的评论
        /// </summary>
        /// <param name="bookId">图书ID</param>
        /// <param name="rating">评论汇总(输出)</param>
        /// <returns>评论列表</returns>
        IList<BookCommentModel> GetBookCommentList(int bookId, BookRatingModel rating = null);

        /// <summary>
        /// 获取指定ISBN图书的评价
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="isbn">图书编码</param>
        /// <param name="rating">评价汇总(输出)</param>
        /// <returns>评价列表</returns>
        IList<BookCommentModel> GetBookCommentList(MissionskyOAEntities dbContext, string isbn, BookRatingModel rating = null);

        /// <summary>
        /// 获取指定ISBN图书的评价
        /// </summary>
        /// <param name="isbn">图书编码</param>
        /// <param name="rating">评价汇总(输出)</param>
        /// <returns>评价列表</returns>
        IList<BookCommentModel> GetBookCommentList(string isbn, BookRatingModel rating = null);
    }
}
