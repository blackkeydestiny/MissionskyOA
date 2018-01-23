using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Pager;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 图书处理接口
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// 分页查询图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的图书</returns>
        IPagedList<BookModel> SearchBooks(SearchBookModel search, int pageIndex, int pageSize);

        /// <summary>
        /// 分页查询图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的图书</returns>
        ListResult<BookModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 根据图书ID 获取图书详细信息
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <param name="userId">用户id</param>
        /// <returns>图书详细信息</returns>
        BookModel GetBookDetail(int bookId, int userId = 0);

        /// <summary>
        /// 查询满足条件的图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>满足条件的图书列表</returns>
        IList<BookModel> GetBookList(SearchBookModel search);

        /// <summary>
        /// 查询满足条件的图书并按照ISBN编码返回
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>满足条件的图书列表</returns>
        IPagedList<BookISBNGroupModel> GetBookListByISBNGroup(SearchBookModel search, int pageIndex, int pageSize);

        /// <summary>
        /// 获取用户已借阅的图书
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>图书列表</returns>
        IList<BookModel> GetUserBorrowedBooks(int userId);

        /// <summary>
        /// 获取用户捐赠的的图书
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>图书列表</returns>
        IList<BookModel> GetUserDonatedBooks(int userId);

        /// <summary>
        /// 查询与用户有关的图书(借阅或捐赠的图书)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>借阅或捐赠的图书</returns>
        IList<BookModel> GetUserBookList(int userId);

        /// <summary>
        /// 获取用户借阅历史记录
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>图书列表</returns>
        IList<BookModel> GetUserBorrowedHistoryBooks(int userId);

        /// <summary>
        /// 借阅图书
        /// </summary>
        /// <param name="model">借阅信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>true or false</returns>
        bool Borrow(BorrowBookModel model, int userId);

        /// <summary>
        /// 转移图书
        /// </summary>
        /// <param name="model">借阅信息</param>
        /// <param name="userId">当前借阅用户</param>
        /// <returns>true or false</returns>
        bool Transfer(BorrowBookModel model, int userId);

        /// <summary>
        /// 归还图书
        /// </summary>
        /// <param name="model">图书评价信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>true or false</returns>
        bool Return(CommentBookModel model, int userId);

        /// <summary>
        /// 添加或修改用户图书评论
        /// </summary>
        /// <param name="model">评论信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>是否评论成功</returns>
        bool Comment(CommentBookModel model, int userId);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">图书评论Id</param>
        /// <returns>是否删除成功</returns>
        bool DeleteComment(int commentId);

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="model">图书信息</param>
        /// <returns>图书Id</returns>
        int Add(BaseBookModel model);
        
        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="bookId">图书Id</param>
        /// <returns></returns>
        bool Delete(int bookId);
        
        /// <summary>
        /// 上传图书封面
        /// </summary>
        /// <param name="cover">图书封面</param>
        /// <returns>上传成功或失败</returns>
        bool UploadCover(AttachmentModel cover);
        /// <summary>
        /// 编辑图书
        /// </summary>
        /// <param name="model">图书基本信息</param>
        /// <param name="bookId">图书信息</param>
        /// <returns>是否编辑成功</returns>
        bool Edit(BaseBookModel model, int bookId);

        /// <summary>
        /// 编辑图书
        /// </summary>
        /// <param name="model">图书基本信息</param>
        /// <param name="bookId">图书信息</param>
        /// <returns>是否编辑成功</returns>
        bool UpdateBookInfo(BookModel model);
    }
}
