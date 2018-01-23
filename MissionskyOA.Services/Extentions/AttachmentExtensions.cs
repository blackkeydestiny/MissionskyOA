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
    /// 附件扩展处理
    /// </summary>
    public static class AttachmentExtensions
    {

        public static AttachmentModel ToModel(this Attachment entity)
        {
            var model = new AttachmentModel()
            {
                Id = entity.Id,
                EntityId = entity.EntityId,
                EntityType = entity.EntityType,
                Name = entity.Name,
                Desc = entity.Desc,
                Content = entity.Content,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }

        public static Attachment ToEntity(this AttachmentModel model)
        {
            var entity = new Attachment()
            {
                Id = model.Id,
                EntityId = model.EntityId,
                EntityType = model.EntityType,
                Name = model.Name,
                Desc = model.Desc,
                Content = model.Content,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }
    }
}
