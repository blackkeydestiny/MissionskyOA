using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 图书借阅处理类
    /// </summary>
    public class BookCommentService : ServiceBase, IBookCommentService
    {
        /// <summary>
        /// 汇总图书评分
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <returns>图书评分</returns>
        public BookRatingModel CountBookRading(int bookId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var rading = new BookRatingModel();

                var entities =
                    dbContext.BookComments.Where(it => it.BookId == bookId).ToList();

                entities.ForEach(it =>
                {
                    if (it.Rating.HasValue)
                    {
                        rading.Count ++;
                        rading.TotalRating += it.Rating.Value;
                    }

                    if (rading.Count > 0)
                    {
                        rading.AverageRating = ((double)rading.TotalRating/rading.Count).Round();
                    }
                });

                return rading;
            }
        }

        /// <summary>
        /// 获取指定ID图书的评价
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="bookId">图书ID</param>
        /// <param name="rating">评价汇总(输出)</param>
        /// <returns>评价列表</returns>
        public IList<BookCommentModel> GetBookCommentList(MissionskyOAEntities dbContext, int bookId,
            BookRatingModel rating = null)
        {
            var comments = new List<BookCommentModel>(); //评价
            GetComments(dbContext, comments, bookId, rating);
            return comments;
        }

        /// <summary>
        /// 获取指定ID图书的评价
        /// </summary>
        /// <param name="bookId">图书ID</param>
        /// <param name="rating">评价汇总(输出)</param>
        /// <returns>评价列表</returns>
        public IList<BookCommentModel> GetBookCommentList(int bookId, BookRatingModel rating = null)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                return GetBookCommentList(dbContext, bookId, rating);
            }
        }

        /// <summary>
        /// 获取指定ISBN图书的评价
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="isbn">图书编码</param>
        /// <param name="rating">评价汇总(输出)</param>
        /// <returns>评价列表</returns>
        public IList<BookCommentModel> GetBookCommentList(MissionskyOAEntities dbContext, string isbn,
            BookRatingModel rating = null)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                Log.Error("图书ISBN无效。");
                throw new InvalidOperationException("图书ISBN无效。");
            }

            var books = dbContext.Books.Where(it => it.ISBN.Equals(isbn));
            var comments = new List<BookCommentModel>(); //评价

            books.ToList().ForEach(book => GetComments(dbContext, comments, book.Id, rating));

            return comments;
        }

        /// <summary>
        /// 获取指定ISBN图书的评价
        /// </summary>
        /// <param name="isbn">图书编码</param>
        /// <param name="rating">评价汇总(输出)</param>
        /// <returns>评价列表</returns>
        public IList<BookCommentModel> GetBookCommentList(string isbn, BookRatingModel rating = null)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                Log.Error("图书ISBN无效。");
                throw new InvalidOperationException("图书ISBN无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                return GetBookCommentList(dbContext, isbn, rating);
            }
        }

        private void GetComments(MissionskyOAEntities dbContext, IList<BookCommentModel> comments, int bookId, BookRatingModel rating = null)
        {
            var entities =
                          dbContext.BookComments.Where(it => it.BookId == bookId).OrderByDescending(it => it.CreatedDate).ToList();

            entities.ForEach(entity =>
            {
                var model = entity.ToModel();

                //评价用户
                var user = dbContext.Users.FirstOrDefault(it => it.Id == model.UserId);
                model.UserName = (user == null ? string.Empty : user.EnglishName);

                comments.Add(model); //评价

                #region 评分汇总
                if (rating != null)
                {
                    if (entity.Rating.HasValue)
                    {
                        rating.Count++;
                        rating.TotalRating += entity.Rating.Value; //记算总分
                    }

                    if (rating.Count > 0)
                    {
                        rating.AverageRating = ((double)rating.TotalRating / rating.Count).Round(); //记算平均分
                    }
                }
                #endregion
            });
        }
    }
}
