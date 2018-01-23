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
    public class BookBorrowService : IBookBorrowService
    {
        /// <summary>
        /// 获取当前图书的借阅详细信息
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <returns>图书借阅详细信息</returns>
        public BookBorrowModel GetBorrowingDetail(int bookId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity =
                    dbContext.BookBorrows.FirstOrDefault(
                        it => it.Status.HasValue && it.Status == (int) UserBorrowStatus.Borrowing && it.BookId == bookId);

                if (entity != null)
                {
                    var model = entity.ToModel();

                    var user = dbContext.Users.FirstOrDefault(it => it.Id == model.UserId); //借阅用户
                    model.UserName = (user != null ? user.EnglishName : string.Empty); //用户名

                    return model;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取用户借阅某本图书的详细信息
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户借阅某图书的详细信息</returns>
        public BookBorrowModel GetUserBorrowDetail(int bookId, int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity =
                    dbContext.BookBorrows.Where(it => it.BookId == bookId && it.UserId == userId).OrderByDescending(item=>item.BorrowDate).FirstOrDefault();

                if (entity != null)
                {
                    var model = entity.ToModel();
                    return model;
                }

                return null;
            }
        }

        /// <summary>
        /// 查询用户的借阅记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户的借阅记录</returns>
        public IList<BookBorrowModel> GetUserBorrowHistory(int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entities = dbContext.BookBorrows.Where(it => it.UserId == userId).ToList();

                var list = new List<BookBorrowModel>();

                entities.ForEach(entity =>
                {
                    var model = entity.ToModel(); //借阅记录

                    var book = dbContext.Books.FirstOrDefault(it => it.Id == model.BookId); //图书
                    model.BookName = (book != null ? book.Name : string.Empty); //图书名

                    list.Add(model);
                });

                return list;
            }
        }

        /// <summary>
        /// 查询图书的借阅记录
        /// </summary>
        /// <param name="bookId">图书ID</param>
        /// <param name="currentBorrow">当前的借阅记录</param>
        /// <returns>图书的借阅记录</returns>
        public IList<BookBorrowModel> GetBookBorrowHistory(int bookId, BookBorrowModel currentBorrow = null)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entities = dbContext.BookBorrows.Where(it => it.BookId == bookId).ToList();

                var list = new List<BookBorrowModel>();

                entities.ForEach(entity =>
                {
                    var model = entity.ToModel(); //借阅记录

                    var user = dbContext.Users.FirstOrDefault(it => it.Id == model.UserId); //借阅用户
                    model.UserName = (user != null ? user.EnglishName : string.Empty); //用户名

                    list.Add(model);
                });

                //获取当前的借阅记录
                if (currentBorrow != null)
                {
                    var userBorrow = list.FirstOrDefault(it => it.Status == UserBorrowStatus.Borrowing);
                    CopyTo(userBorrow, currentBorrow); //复制
                }

                return list;
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        private void CopyTo(BookBorrowModel source, BookBorrowModel target)
        {
            if (source != null && target != null)
            {
                target.Id = source.Id;
                target.UserId = source.UserId;
                target.UserName = source.UserName;
                target.BookId = source.BookId;
                target.BookName = source.BookName;
                target.Status = source.Status;
                target.ReturnDate = source.ReturnDate;
                target.BorrowDate = source.BorrowDate;
            }
        }
    }
}
