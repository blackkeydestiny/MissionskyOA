using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 图书处理类
    /// </summary>
    public class BookService : ServiceBase, IBookService
    {
        private readonly IBookBorrowService _borrowService = new BookBorrowService();
        private readonly IBookCommentService _commentService = new BookCommentService();
        private readonly IUserService _userService = new UserService();
        private readonly IAttachmentService _attachmentService = new AttachmentService();
        private readonly INotificationService _notificationService = new NotificationService();

        /// <summary>
        /// 分页查询图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的图书</returns>
        public IPagedList<BookModel> SearchBooks(SearchBookModel search, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = DoSearch(dbContext, search); //查询
                var result = new List<BookModel>();

                query.ToList().ForEach(entity => result.Add(DoFill(entity.ToModel())));
                result = result.OrderByDescending(it => it.Rating.AverageRating).ToList();

                return new PagedList<BookModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 查询满足条件的图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>满足条件的图书列表</returns>
        public IList<BookModel> GetBookList(SearchBookModel search)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = DoSearch(dbContext, search); //查询
                var list = new List<BookModel>();

                query.ToList().ForEach(entity => list.Add(DoFill(entity.ToModel())));
                list = list.OrderByDescending(it => it.Rating.AverageRating).ToList();

                return list;
            }
        }

        public IPagedList<BookISBNGroupModel> GetBookListByISBNGroup(SearchBookModel search, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = DoSearch(dbContext, search); //查询
                var groupQuery = query.GroupBy(x => x.ISBN).Select(group =>
                new
                {
                    ISBN = group.Key,
                    books = group.Where(it => it.Id != null)
                });

                List<BookISBNGroupModel> bookGroupList = new List<BookISBNGroupModel>();

                groupQuery.ToList().ForEach(item =>
                {
                    BookISBNGroupModel temp = new BookISBNGroupModel()
                    {
                        books = new List<BookModel>()
                    };
                    temp.ISBN = item.ISBN;
                    item.books.ToList().ForEach(i =>
                    {
                        temp.books.Add(DoFill(i.ToModel()));
                    });
                    temp.amount = item.books.ToList().Count;
                    temp.books = temp.books.OrderByDescending(it => it.Rating.AverageRating).ToList();
                    bookGroupList.Add(temp);
                });
                return new PagedList<BookISBNGroupModel>(bookGroupList, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 获取用户已借阅的图书
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>图书列表</returns>
        public IList<BookModel> GetUserBorrowedBooks(int userId)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("用户ID无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var list = new List<BookModel>();

                var userBorrows = _borrowService.GetUserBorrowHistory(userId); //用户借阅记录

                if (userBorrows != null)
                {
                    userBorrows.ToList().ForEach(borrow =>
                    {
                        if (!list.Exists(it => it.Id == borrow.BookId)) //消除重复
                        {
                            var book = dbContext.Books.FirstOrDefault(entity => entity.Id == borrow.BookId);

                            if (book != null)
                            {
                                list.Add(DoFill(book.ToModel(), userId));
                            }
                        }

                    });
                }

                return list;
            }
        }

        /// <summary>
        /// 获取用户借阅历史记录
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>图书列表</returns>
        public IList<BookModel> GetUserBorrowedHistoryBooks(int userId)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("用户ID无效。");
            }
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = new List<BookModel>();
                var userBorrows = _borrowService.GetUserBorrowHistory(userId); //用户借阅记录
                if (userBorrows != null)
                {
                    userBorrows.ToList().ForEach(borrow =>
                    {
                        var book = dbContext.Books.FirstOrDefault(entity => entity.Id == borrow.BookId);
                        if (book != null)
                        {
                            list.Add(DoFill(book.ToModel(), userId));
                        }
                    });
                }
                return list;
            }
        }

        /// <summary>
        /// 获取用户捐赠的的图书
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>图书列表</returns>
        public IList<BookModel> GetUserDonatedBooks(int userId)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("用户ID无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var list = new List<BookModel>();

                var donatedBooks = dbContext.Books.Where(it => it.Donor.HasValue && it.Donor.Value == userId); //用户借阅记录

                donatedBooks.ToList().ForEach(book => list.Add(DoFill(book.ToModel(), userId))); //获取详细

                return list;
            }
        }

        /// <summary>
        /// 查询与用户有关的图书(借阅或捐赠的图书)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>借阅或捐赠的图书</returns>
        public IList<BookModel> GetUserBookList(int userId)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("用户ID无效。");
            }

            var books = new List<BookModel>(); //与用户相关的图书

            var borrowedBooks = GetUserBorrowedBooks(userId); //用户借阅的图书
            if (borrowedBooks != null)
            {
                borrowedBooks = borrowedBooks.OrderByDescending(it => it.CurrentUserBorrow.ReturnDate).ToList();
                books.AddRange(borrowedBooks.ToList());
            }

            var donatedBooks = GetUserDonatedBooks(userId); //用户捐赠的的图书
            if (donatedBooks != null)
            {
                books.AddRange(donatedBooks.ToList());
            }

            var list = new List<BookModel>(); //过滤重复
            books.ForEach(book =>
            {
                if (!list.Exists(it => it.Id == book.Id))
                {
                    list.Add(book);
                }
            });

            return list;
        }

        /// <summary>
        /// 根据图书ID 获取图书详细信息
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <param name="userId">用户id</param>
        /// <returns>图书详细信息</returns>
        public BookModel GetBookDetail(int bookId, int userId = 0)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Books.FirstOrDefault(it => it.Id == bookId);

                if (entity == null)
                {
                    Log.Error(string.Format("找不到指定ID的图书, Id: {0}", bookId));
                    throw new KeyNotFoundException("找不到指定ID的图书。");
                }
                return DoFill(entity.ToModel(), userId, true);
            }
        }

        /// <summary>
        /// 借阅图书
        /// </summary>
        /// <param name="model">借阅信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>true or false</returns>
        public bool Borrow(BorrowBookModel model, int userId)
        {
            if (model == null)
            {
                throw new InvalidOperationException("图书借阅信息无效。");
            }
            
            using (var dbContext = new MissionskyOAEntities())
            {
                if (DoBorrow(dbContext, model, userId))
                {
                    dbContext.SaveChanges();
                }
            }

            return true;
        }

        /// <summary>
        /// 转移图书
        /// </summary>
        /// <param name="model">借阅信息</param>
        /// <param name="userId">当前借阅用户</param>
        /// <returns>true or false</returns>
        public bool Transfer(BorrowBookModel model, int userId)
        {
            if (model == null)
            {
                throw new InvalidOperationException("图书借阅信息无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //当前用户的借阅信息失效
                var current =
                    dbContext.BookBorrows.FirstOrDefault(
                        it =>
                            it.BookId == model.BookId && it.UserId == userId &&
                            it.Status == (int)UserBorrowStatus.Borrowing);

                if (current != null)
                {
                    current.Status = (int) UserBorrowStatus.Returned;
                }

                //转移图书到其它用户
                if (model.Reader.HasValue && DoBorrow(dbContext, model, model.Reader.Value))
                {
                    dbContext.SaveChanges();
                }
            }

            return true;
        }

        /// <summary>
        /// 归还图书
        /// </summary>
        /// <param name="model">图书评价信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>true or false</returns>
        public bool Return(CommentBookModel model, int userId)
        {
            if (model == null)
            {
                throw new InvalidOperationException("图书评价信息无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var book = dbContext.Books.FirstOrDefault(it => it.Id == model.BookId);
                if (book == null)
                {
                    throw new InvalidOperationException("借阅图书不存在。");
                }

                var borrow =
                    dbContext.BookBorrows.FirstOrDefault(
                        it =>
                            it.UserId == userId && it.BookId == model.BookId &&
                            it.Status == (int) UserBorrowStatus.Borrowing);

                if (borrow == null)
                {
                    throw new InvalidOperationException("借阅记录不存在。");
                }

                if (!book.Reader.HasValue || book.Reader.Value != userId)
                {
                    throw new InvalidOperationException("图书未被当前用户借出。");
                }

                model.ValidRating(); //验证评分

                //添加评价
                var comment = new BookComment()
                {
                    BookId = model.BookId,
                    UserId = userId,
                    Comment = model.Comment,
                    Rating = model.Rating,
                    CreatedDate = DateTime.Now
                };

                dbContext.BookComments.Add(comment);

                //更新借书记录
                borrow.Status = (int) UserBorrowStatus.Returned;
                borrow.ReturnDate = DateTime.Now; //实际归还时间

                //更新图书状态
                book.Status = (int) BookStatus.Stored;
                book.Reader = null;
                
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 添加或修改用户图书评论
        /// </summary>
        /// <param name="model">评论信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>是否评论成功</returns>
        public bool Comment(CommentBookModel model, int userId)
        {
            if (model == null)
            {
                throw new InvalidOperationException("图书评价信息无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {

                var user = dbContext.Users.SqlQuery("select * from [user] where id = 2");

                if (user != null)
                {
                    
                }

                var book = dbContext.Books.FirstOrDefault(it => it.Id == model.BookId);
                if (book == null)
                {
                    throw new InvalidOperationException("图书不存在。");
                }

                model.ValidRating(); //验证评分

                var comment =
                    dbContext.BookComments.FirstOrDefault(it => model.CommentId.HasValue && it.Id == model.CommentId); //获取评论

                if (comment != null) //编辑评论
                {
                    comment.Rating = model.Rating;
                    comment.Comment = model.Comment;
                }
                else //添加评价
                {
                    comment = new BookComment()
                    {
                        BookId = model.BookId,
                        UserId = userId,
                        Comment = model.Comment,
                        Rating = model.Rating,
                        CreatedDate = DateTime.Now
                    };

                    dbContext.BookComments.Add(comment);
                }

                //dbContext.SaveChanges();
            }

            return true;
        }
        
        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">图书评论Id</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteComment(int commentId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var comment = dbContext.BookComments.FirstOrDefault(it => it.Id == commentId);

                if (comment == null)
                {
                    throw new KeyNotFoundException("图书评论不存在。");
                }

                dbContext.BookComments.Remove(comment);
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="model">图书基本信息</param>
        /// <returns>是否添加成功</returns>
        public int Add(BaseBookModel model)
        {
            if (model == null)
            {
                throw new InvalidOperationException("图书信息无效。");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new InvalidOperationException("图书名称不能为空。");
            }

            if (model.Source == BookSource.None)
            {
                throw new InvalidOperationException("未选择图书来源。");
            }

            var book = new Book()
            {
                Name = model.Name,
                ISBN = model.ISBN,
                Author = model.Author,
                Desc = model.Desc,
                Source = (int)model.Source,
                Donor = model.Donor,
                Type = model.Type,
                Status = (int)BookStatus.Stored,
                CreatedDate = DateTime.Now,
                BarCode = Guid.NewGuid().ToString()
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                book = dbContext.Books.Add(book);
                dbContext.SaveChanges();
            }

            return book.Id;
        }

        /// <summary>
        /// 编辑图书
        /// </summary>
        /// <param name="model">图书基本信息</param>
        /// <param name="bookId">图书信息</param>
        /// <returns>是否编辑成功</returns>
        public bool Edit(BaseBookModel model, int bookId)
        {
            if (model == null)
            {
                throw new InvalidOperationException("图书信息无效。");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new InvalidOperationException("图书名称不能为空。");
            }

            if (model.Source == BookSource.None)
            {
                throw new InvalidOperationException("未选择图书来源。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var book = new Book()
                {
                    Name = model.Name,
                    ISBN = model.ISBN,
                    Author = model.Author,
                    Desc = model.Desc,
                    Source = (int)model.Source,
                    Donor = model.Donor,
                    Type = model.Type,
                    Status = (int)BookStatus.Stored,
                    CreatedDate = DateTime.Now
                };

                dbContext.Books.Add(book);
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="bookId">图书Id</param>
        /// <returns>是否删除成功</returns>
        public bool Delete(int bookId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var sql = @"DELETE BookComment WHERE BookId={0}; --删除评论
                            DELETE BookBorrow WHERE BookId={0}; --删除借阅信息
                            DELETE Book WHERE Id={0}; --删除图书";

                dbContext.Database.ExecuteSqlCommand(string.Format(sql, bookId));
            }

            return true;
        }
        
        /// <summary>
        /// 上传图书封面
        /// </summary>
        /// <param name="cover">图书封面</param>
        /// <returns>上传成功或失败</returns>
        public bool UploadCover(AttachmentModel cover)
        {
            return _attachmentService.Upload(cover);
        }

        /// <summary>
        /// 图书查询
        /// </summary>
        /// <param name="dbContext">数据库上下文对象</param>
        /// <param name="search">查询条件</param>
        /// <returns>满足条件的图收列表</returns>
        private IEnumerable<Book> DoSearch(MissionskyOAEntities dbContext, SearchBookModel search)
        {
            var query = dbContext.Books.AsEnumerable();

            //状态查询
            if (search != null && search.Status != BookStatus.None)
            {
                query =
                    query.Where(
                        it =>
                            (!it.Status.HasValue && search.Status == BookStatus.Stored) ||
                            (it.Status.HasValue && search.Status == (BookStatus)it.Status));
            }

            //项目查询
            if (search != null && !string.IsNullOrEmpty(search.Name))
            {
                query = query.Where(it => !string.IsNullOrEmpty(it.Name) && it.Name.Contains(search.Name));
            }

            //二维码
            if (search != null && !string.IsNullOrEmpty(search.BarCode))
            {
                query = query.Where(it => !string.IsNullOrEmpty(it.BarCode) && it.BarCode.Equals(search.BarCode, StringComparison.InvariantCultureIgnoreCase));
            }

            //二维码
            if (search != null && !string.IsNullOrEmpty(search.ISBN))
            {
                query = query.Where(it => !string.IsNullOrEmpty(it.ISBN) && it.ISBN.Equals(search.ISBN, StringComparison.InvariantCultureIgnoreCase));
            }
            //状态
            if (search != null && search.Source != BookSource.None)
            {
                query =
                    query.Where(
                        it =>
                            (!it.Source.HasValue && search.Source == BookSource.Purchase) ||
                            (it.Source.HasValue && it.Source == (int) search.Source));
            }
            return query;
        }

        /// <summary>
        /// 获取图书相关信息
        /// </summary>
        /// <param name="book">图书</param>
        /// <param name="userId">用户Id</param>
        /// <param name="isDetail">是否加载详细信息</param>
        /// <returns>图书</returns>
        public BookModel DoFill(BookModel book, int userId = 0, bool isDetail = false)
        {
            if (book == null)
            {
                Log.Error("图书对象为空。");
                throw new NullReferenceException("图书对象为空。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //图书当前借阅信息
                book.BorrowBook = _borrowService.GetBorrowingDetail(book.Id);
                if (book.BorrowBook != null)
                {
                    book.Reader = book.BorrowBook.UserId;
                }

                //获取用户借阅某本图书的详细信息
                if (userId > 0)
                {
                    book.CurrentUserBorrow = _borrowService.GetUserBorrowDetail(book.Id, userId);
                }

                //捐赠人
                if (book.Donor.HasValue)
                {
                    var donor = _userService.GetUserDetail(book.Donor.Value);
                    book.DonorName = (donor == null ? null : donor.EnglishName); //捐赠人姓名
                }

                //评价
                book.Rating = new BookRatingModel();
                var comments = _commentService.GetBookCommentList(dbContext, book.ISBN, book.Rating);
                book.BookComments = isDetail ? comments : null;
                book.BookCommentCount = comments.Count;

                //附件
                book.BookCovers = _attachmentService.GetAttathcmentIds(dbContext, book.Id, Constant.ATTACHMENT_TYPE_BOOK_COVER);

                return book;
            }
        }

        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="dbContext">数据库上下文对象</param>
        /// <param name="model">图书借阅信息</param>
        /// <param name="userId">借阅用户</param>
        /// <returns>是否借阅成功</returns>
        private bool DoBorrow(MissionskyOAEntities dbContext, BorrowBookModel model, int userId)
        {
            if (model == null)
            {
                throw new InvalidOperationException("无效借阅信息");
            }

            if (model.ReturnDate.CompareTo(DateTime.Now) < 1)
            {
                throw new InvalidOperationException("归还日期必须大于当前日期");
            }
            bool isAllowBrrow = false;
            if (model.BookId != null && model.BookId > 0)
            {
                var book = dbContext.Books.FirstOrDefault(it => it.Id == model.BookId);
                if (book == null)
                {
                    throw new InvalidOperationException("无此图书");
                }
                isAllowBrrow = AllowBorrow(book.ToModel());
                book.Status = (int)BookStatus.Borrowed;
                book.Reader = userId;
               
                if (isAllowBrrow)
                {
                    //借阅
                    var bookBorrow = new BookBorrow()
                    {
                        BookId = model.BookId,
                        UserId = userId,
                        BorrowDate = DateTime.Now,
                        ReturnDate = model.ReturnDate,
                        Status = (int)UserBorrowStatus.Borrowing
                    };

                    dbContext.BookBorrows.Add(bookBorrow);
                    dbContext.SaveChanges();
                    return true;
                }

            }
            if (!string.IsNullOrEmpty(model.BarCode))
            {
                var book = dbContext.Books.Where(it => !string.IsNullOrEmpty(it.BarCode) && it.BarCode.Equals(model.BarCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (book == null)
                {
                    throw new InvalidOperationException("无此图书");
                }
                isAllowBrrow = AllowBorrow(book.ToModel());
                book.Status = (int)BookStatus.Borrowed;
                book.Reader = userId;
                if (isAllowBrrow)
                {
                    //借阅
                    var bookBorrow = new BookBorrow()
                    {
                        BookId = book.Id,
                        UserId = userId,
                        BorrowDate = DateTime.Now,
                        ReturnDate = model.ReturnDate,
                        Status = (int)UserBorrowStatus.Borrowing
                    };
                    dbContext.BookBorrows.Add(bookBorrow);
                    dbContext.SaveChanges();
                    return true;
                }
            }
          

            return false;
        }

        /// <summary>
        /// 是否允许被借出
        /// </summary>
        /// <param name="book">图书类型</param>
        /// <returns>true or false</returns>
        private bool AllowBorrow(BookModel book)
        {
            if (book == null)
            {
                throw new InvalidOperationException("借阅图书信息无效。");
            }

            if (book.Status == BookStatus.Lost || book.Status == BookStatus.Removed || book.Status == BookStatus.None)
            {
                throw new InvalidOperationException("借阅图书不存在。");
            }

            if (book.Reader.HasValue || book.Status == BookStatus.Borrowed)
            {
                throw new InvalidOperationException("借阅图书已经被借出。");
            }

            return true;
        }

        /// <summary>
        /// 分页查询图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的图书</returns>
        public ListResult<BookModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Books.AsEnumerable();

                //if (sort != null)
                //{

                //}

                var count = list.Count();
                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<BookModel> result = new ListResult<BookModel>();
                result.Data = new List<BookModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }
        
        /// <summary>
        /// 更新书籍信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool UpdateBookInfo(BookModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.Books.FirstOrDefault(it => it.Id == model.Id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("找不到此图书");
                }
                entity.Name = model.Name;
                entity.ISBN = model.ISBN;
                entity.Author = model.Author;
                entity.Desc = model.Desc;
                entity.Source = (int)model.Source;
                entity.Donor = model.Donor;
                entity.Type = model.Type;
                entity.Status = (int)BookStatus.Stored;
                dbcontext.SaveChanges();
                return true;
            }
        }

    }
}
