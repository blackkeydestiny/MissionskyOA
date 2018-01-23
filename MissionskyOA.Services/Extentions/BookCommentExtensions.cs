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
    /// 图书评价扩展处理
    /// </summary>
    public static class BookCommentExtensions
    {
        public static BookCommentModel ToModel(this BookComment entity)
        {
            var model = new BookCommentModel()
            {
                Id = entity.Id,
                BookId = entity.BookId,
                UserId = entity.UserId,
                Comment = entity.Comment,
                Rating = entity.Rating.HasValue ? entity.Rating.Value : 0,
                CreatedDate = entity.CreatedDate
            };

            return model;
        }

        public static BookComment ToEntity(this BookCommentModel model)
        {
            var entity = new BookComment()
            {
                Id = model.Id,
                BookId = model.BookId,
                UserId = model.UserId,
                Comment = model.Comment,
                Rating = model.Rating,
                CreatedDate = model.CreatedDate
            };

            return entity;
        }
    }
}
