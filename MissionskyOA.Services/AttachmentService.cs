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
    /// 附件处理类
    /// </summary>
    public class AttachmentService : IAttachmentService
    {
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="entityId">所属对象Id</param>
        /// <param name="entityType">所属对象类型</param>
        /// <returns>附件列列</returns>
        public IList<AttachmentModel> GetAttathcmentList(int entityId, string entityType)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = new List<AttachmentModel>();

                var query  = dbContext.Attachments.Where(
                    it =>
                        it.EntityId == entityId &&
                        it.EntityType.Equals(entityType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(it => it.CreatedTime);

                query.ToList().ForEach(it =>
                {
                    if (it != null)
                    {
                        list.Add(it.ToModel());
                    }
                });

                return list;
            }
        }

        /// <summary>
        /// 获取附件Id
        /// </summary>
        /// <param name="entityId">所属对象Id</param>
        /// <param name="entityType">所属对象类型</param>
        /// <returns>附件Id</returns>
        public IList<int> GetAttathcmentIds(int entityId, string entityType)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = new List<int>();

                var query = dbContext.Attachments.Where(
                    it =>
                        it.EntityId == entityId &&
                        it.EntityType.Equals(entityType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(it => it.CreatedTime);

                query.ToList().ForEach(it =>
                {
                    if (it != null)
                    {
                        list.Add(it.Id);
                    }
                });

                return list;
            }
        }

        /// <summary>
        /// 获取附件Id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="entityId">所属对象Id</param>
        /// <param name="entityType">所属对象类型</param>
        /// <returns>附件Id</returns>
        public IList<int> GetAttathcmentIds(MissionskyOAEntities dbContext, int entityId, string entityType)
        {
            var list = new List<int>();

            var query = dbContext.Attachments.Where(
                it =>
                    it.EntityId == entityId &&
                    it.EntityType.Equals(entityType, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(it => it.CreatedTime);

            query.ToList().ForEach(it =>
            {
                if (it != null)
                {
                    list.Add(it.Id);
                }
            });

            return list;
        }

        /// <summary>
        /// 获取附件详细
        /// </summary>
        /// <param name="id">附件Id</param>
        /// <returns>附件详细</returns>
        public AttachmentModel GetAttathcmentDetail(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Attachments.FirstOrDefault(it => it.Id == id);

                if (entity != null)
                {
                    return entity.ToModel();
                }

                return null;
            }
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="attachment">附件信息</param>
        /// <returns>上传成功或失败</returns>
        public bool Upload(AttachmentModel attachment)
        {
            if (attachment == null || attachment.Content == null || string.IsNullOrEmpty(attachment.EntityType) ||
                attachment.EntityId < 1)
            {
                throw new InvalidOperationException("附件信息无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                dbContext.Attachments.Add(attachment.ToEntity());
                dbContext.SaveChanges();

                return true;
            }
        }
    }
}
