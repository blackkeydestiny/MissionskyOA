using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class AvatarExtensions
    {
        public static Avatar ToEntity(this AvatarModel model)
        {
            var entity = new Avatar()
            {
                UserId = model.UserId,
                FileName = model.FileName,
                Content = model.Content,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static AvatarModel ToModel(this Avatar entity)
        {
            var model = new AvatarModel()
            {
                UserId = entity.UserId,
                FileName = entity.FileName,
                Content = entity.Content,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }

    }
}
