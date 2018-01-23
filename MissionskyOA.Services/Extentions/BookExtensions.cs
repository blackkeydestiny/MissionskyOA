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
    /// 图书扩展处理
    /// </summary>
    public static class BookExtensions
    {
        public static BookModel ToModel(this Book entity)
        {
            var model = new BookModel()
            {
                Id = entity.Id,
                Type = entity.Type,
                Author = entity.Author,
                Desc = entity.Desc,
                Name = entity.Name,
                ISBN = entity.ISBN,
                BarCode = entity.BarCode,
                Donor = entity.Donor,
                Reader = entity.Reader,
                Source = entity.Source.HasValue ? (BookSource) entity.Source.Value : BookSource.None,
                Status = entity.Status.HasValue ? (BookStatus) entity.Status.Value : BookStatus.None,
                CreatedDate = entity.CreatedDate
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.Donor);
                if (userEntity != null)
                {
                    model.DonorName = userEntity.EnglishName;
                }
            }

            return model;
        }

        public static Book ToEntity(this BookModel model)
        {
            var entity = new Book()
            {
                Id = model.Id,
                Type = model.Type,
                Author = model.Author,
                Desc = model.Desc,
                Name = model.Name,
                ISBN = model.ISBN,
                BarCode = model.BarCode,
                Donor = model.Donor,
                Reader = model.Reader,
                Source = model.Source == BookSource.None ? (int) BookSource.Purchase : (int) model.Source,
                Status = model.Status == BookStatus.None ? (int) BookStatus.Stored : (int) model.Status,
                CreatedDate = model.CreatedDate
            };

            return entity;
        }

        /// <summary>
        /// 验证评分是否有效
        /// </summary>
        /// <param name="model">评论</param>
        /// <returns>true or false</returns>
        public static bool ValidRating(this CommentBookModel model)
        {
            if (model.Rating > 5)
            {
                throw new InvalidOperationException("最高评分5分。");
            }

            return true;
        }
    }
}
