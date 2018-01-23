using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 图书借阅处理接口
    /// </summary>
    public interface IBookBorrowService
    {
        /// <summary>
        /// 获取当前图书的借阅详细信息
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <returns>图书借阅详细信息</returns>
        BookBorrowModel GetBorrowingDetail(int bookId);

        /// <summary>
        /// 获取用户借阅某本图书的详细信息
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户借阅某图书的详细信息</returns>
        BookBorrowModel GetUserBorrowDetail(int bookId, int userId);

        /// <summary>
        /// 查询用户的借阅记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户的借阅记录</returns>
        IList<BookBorrowModel> GetUserBorrowHistory(int userId);

        /// <summary>
        /// 查询图书的借阅记录
        /// </summary>
        /// <param name="bookId">图书ID</param>
        /// <param name="currentBorrow">当前的借阅记录</param>
        /// <returns>图书的借阅记录</returns>
        IList<BookBorrowModel> GetBookBorrowHistory(int bookId, BookBorrowModel currentBorrow = null);
    }
}
