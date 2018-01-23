using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 图书管理
    /// </summary>
    [RoutePrefix("api/books")]
    public class BookController : BaseController
    {
        private IBookService BookService { get; set; }
        private IBookBorrowService BookBorrowService { get; set; }
        private IAttachmentService AttachmentService { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public BookController(IBookService bookService, IBookBorrowService bookBorrowService, IAttachmentService attachmentService)
        {
            this.BookService = bookService;
            this.BookBorrowService = bookBorrowService;
            this.AttachmentService = attachmentService;
        }

        /// <summary>
        /// 根据图书ID获取图书详细信息
        /// </summary>
        /// <param name="id">图书id</param>
        /// <returns>图书详细信息</returns>
        [Route("{id}")]
        [HttpGet]
        public ApiResponse<BookModel> GetBookDetail(int id)
        {
            if (id < 1)
            {
                throw new Exception("无效的图书ID。");
            }

            var book = this.BookService.GetBookDetail(id,this.Member.Id);

            var response = new ApiResponse<BookModel>()
            {
                Result = book
            };

            return response;
        }

        /// <summary>
        /// 查询满足条件的所有图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>图书列表</returns>
        [Route("all")]
        [HttpPost]
        public ApiListResponse<BookModel> GetBookList(SearchBookModel search)
        {
            var books = this.BookService.GetBookList(search);

            var response = new ApiListResponse<BookModel>()
            {
                Result = books.ToList()
            };

            return response;
        }

        /// <summary>
        /// 查询满足条件的所有图书并按照ISBN编码group返回
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>图书列表</returns>
        [Route("ISBNGroup")]
        [HttpPost]
        public ApiPagingListResponse<BookISBNGroupModel> GetBookListByISBNGroup(SearchBookModel search, int pageIndex = 0, int pageSize = 25)
        {
            var query = this.BookService.GetBookListByISBNGroup(search, pageIndex, pageSize);

            //查询结果
            var result = new PaginationModel<BookISBNGroupModel>();
            result.Result = query.Result;

            //分页
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<BookISBNGroupModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 分页查询图书
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>图书分页列表</returns>
        [Route("search")]
        [HttpPost]
        public ApiPagingListResponse<BookModel> SearchBooks(SearchBookModel search, int pageIndex = 0, int pageSize = 25)
        {
            //查询满足条件的图书
            var query = this.BookService.SearchBooks(search, pageIndex, pageSize);

            //查询结果
            var result = new PaginationModel<BookModel>();
            result.Result = query.Result;

            //分页
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<BookModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }
        
        /// <summary>
        /// 获取用户的借阅记录
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>用户借阅记录</returns>
        [Route("record")]
        [HttpGet]
        public ApiListResponse<BookModel> GetUserBorrowHistory()
        {
            var list = this.BookService.GetUserBorrowedHistoryBooks(this.Member.Id);

            var response = new ApiListResponse<BookModel>()
            {
                Result = list.ToList()
            };

            return response;
        }

        /// <summary>
        /// 获取图书的借阅历史记录
        /// </summary>
        /// <param name="bookId">图书id</param>
        /// <returns>图书借阅历史记录</returns>
        [Route("history/{bookId}")]
        [HttpGet]
        public ApiListResponse<BookBorrowModel> GetBookBorrowHistory(int bookId)
        {
            if (bookId < 1)
            {
                throw new Exception("无效的图书ID。");
            }

            var lsit = this.BookBorrowService.GetBookBorrowHistory(bookId);

            var response = new ApiListResponse<BookBorrowModel>()
            {
                Result = lsit.ToList()
            };

            return response;
        }

        /// <summary>
        /// 查询与用户有关的图书(借阅或捐赠的图书)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>借阅或捐赠的图书</returns>
        [Route("user/{userId}")]
        [HttpGet]
        public ApiListResponse<BookModel> GetUserBookList(int userId)
        {
            if (userId < 1)
            {
                throw new Exception("无效的用户ID。");
            }

            var lsit = this.BookService.GetUserBookList(userId);

            var response = new ApiListResponse<BookModel>()
            {
                Result = lsit.ToList()
            };

            return response;
        }
        
        /// <summary>
        /// 查询与当前用户有关的图书(借阅或捐赠的图书)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>借阅或捐赠的图书</returns>
        [Route("user/current")]
        [HttpGet]
        public ApiListResponse<BookModel> GetCurrentUserBookList()
        {
            var lsit = this.BookService.GetUserBookList(this.Member.Id);

            var response = new ApiListResponse<BookModel>()
            {
                Result = lsit.ToList()
            };

            return response;
        }

        /// <summary>
        /// 下载图书封面
        /// </summary>
        /// <param name="id">图书封面附件Id</param>
        /// <returns>图书封面文件流</returns>
        [Route("download/cover/{id}")]
        [HttpGet]
        public HttpResponseMessage Download(int id)
        {
            var model = this.AttachmentService.GetAttathcmentDetail(id);
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            if (model != null)
            {
                result.Content = new ByteArrayContent(model.Content);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            }
            else
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }

        /// <summary>
        /// 上传图书封面
        /// </summary>
        /// <param name="bookId">图书Id</param>
        /// <returns>是否上传成功</returns>
        [Route("upload/cover/{bookId}")]
        [HttpPost]
        public ApiResponse<bool> Upload(int bookId)
        {
            return AttachmentController.Upload(bookId, Constant.ATTACHMENT_TYPE_BOOK_COVER);
        }

        /// <summary>
        /// 用户借阅图书,根据图书id或者二维码借阅
        /// </summary>
        /// <param name="model">借阅信息</param>
        /// <returns>是否借阅成功</returns>
        [Route("borrow")]
        [HttpPut]
        public ApiResponse<bool> Borrow(BorrowBookModel model)
        {
            if (model == null)
            {
                throw new Exception("无效的借阅信息。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = this.BookService.Borrow(model, this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 用户转移图书
        /// </summary>
        /// <param name="model">转移信息</param>
        /// <returns>是否转移成功</returns>
        [Route("transfer")]
        [HttpPut]
        public ApiResponse<bool> Transfer(BorrowBookModel model)
        {
            if (model == null)
            {
                throw new Exception("无效的借阅信息。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = this.BookService.Transfer(model, this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 用户归还图书
        /// </summary>
        /// <param name="model">借阅信息</param>
        /// <returns>是否归还成功</returns>
        [Route("return")]
        [HttpPut]
        public ApiResponse<bool> Return(CommentBookModel model)
        {
            if (model == null)
            {
                throw new Exception("无效的借阅信息。");
            }
            if(!string.IsNullOrEmpty(model.Comment))
            {
                if(model.Rating==null||model.Rating==0)
                {
                    throw new Exception("请添加评分");
                }
            }
            if (model.Rating != null && model.Rating != 0)
            {
                if (string.IsNullOrEmpty(model.Comment))
                {
                    throw new Exception("请添加评论");
                }
            }
            var response = new ApiResponse<bool>()
            {
                Result = this.BookService.Return(model, this.Member.Id)
            };

            return response;
        }
        
        /// <summary>
        /// 添加或修改用户图书评论
        /// </summary>
        /// <param name="model">评论信息</param>
        /// <returns>是否评论成功</returns>
        [Route("comment")]
        [HttpPut]
        public ApiResponse<bool> Comment(CommentBookModel model)
        {
            if (model == null)
            {
                throw new Exception("无效的评论信息。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = this.BookService.Comment(model, this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">图书评论Id</param>
        /// <returns>是否删除成功</returns>
        [Route("comment/delete/{id}")]
        [HttpPut]
        public ApiResponse<bool> DeleteComment(int id)
        {
            var response = new ApiResponse<bool>()
            {
                Result = this.BookService.DeleteComment(id)
            };

            return response;
        }

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="model">图书信息</param>
        /// <returns>是否添加成功</returns>
        [Route("add")]
        [HttpPost]
        public ApiResponse<int> Add(BaseBookModel model)
        {
            if (model == null)
            {
                throw new Exception("无效的图书信息。");
            }

            var response = new ApiResponse<int>()
            {
                Result = this.BookService.Add(model)
            };

            return response;
        }

        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="bookId">图书Id</param>
        /// <returns>是否删除成功</returns>
        [Route("delete/{bookId}")]
        [HttpGet]
        public ApiResponse<bool> Add(int bookId)
        {
            if (bookId < 1)
            {
                throw new Exception("无效的图书ID。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = this.BookService.Delete(bookId)
            };

            return response;
        }
    }
}
