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
    /// 图书借阅扩展处理
    /// </summary>
    public static class BookBorrowExtensions
    {
        public static BookBorrowModel ToModel(this BookBorrow entity)
        {
            var model = new BookBorrowModel()
            {
                Id = entity.Id,
                BookId = entity.BookId,
                UserId = entity.UserId,
                Status = entity.Status.HasValue ? (UserBorrowStatus) entity.Status.Value : UserBorrowStatus.None,
                BorrowDate = entity.BorrowDate,
                ReturnDate = entity.ReturnDate
            };

            return model;
        }

        public static BookBorrow ToEntity(this BookBorrowModel model)
        {
            var entity = new BookBorrow()
            {
                Id = model.Id,
                BookId = model.BookId,
                UserId = model.UserId,
                Status = (int) model.Status,
                BorrowDate = model.BorrowDate,
                ReturnDate = model.ReturnDate
            };

            return entity;
        }
    }
}
