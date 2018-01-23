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
    /// 附件处理接口
    /// </summary>
    public interface IAttachmentService
    {
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="entityId">所属对象Id</param>
        /// <param name="entityType">所属对象类型</param>
        /// <returns>附件列表</returns>
        IList<AttachmentModel> GetAttathcmentList(int entityId, string entityType);

        /// <summary>
        /// 获取附件Id
        /// </summary>
        /// <param name="entityId">所属对象Id</param>
        /// <param name="entityType">所属对象类型</param>
        /// <returns>附件Id</returns>
        IList<int> GetAttathcmentIds(int entityId, string entityType);

        /// <summary>
        /// 获取附件Id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="entityId">所属对象Id</param>
        /// <param name="entityType">所属对象类型</param>
        /// <returns>附件Id</returns>
        IList<int> GetAttathcmentIds(MissionskyOAEntities dbContext, int entityId, string entityType);

        /// <summary>
        /// 获取附件详细
        /// </summary>
        /// <param name="id">附件Id</param>
        /// <returns>附件详细</returns>
        AttachmentModel GetAttathcmentDetail(int id);

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="attachment">附件信息</param>
        /// <returns>上传成功或失败</returns>
        bool Upload(AttachmentModel attachment);
    }
}
