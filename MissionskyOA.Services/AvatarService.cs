using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Email;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 头像管理
    /// </summary>
    public class AvatarService : IAvatarService
    {
        /// <summary>
        /// 保存头像
        /// </summary>
        /// <param name="avatar">头像详细信息</param>
        /// <returns>true or false</returns>
        public bool Save(AvatarModel avatar)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Avatars.Include(it => it.User).FirstOrDefault(item => (item.UserId == avatar.UserId));
                if (entity != null)
                {
                    entity.FileName = avatar.FileName;
                    entity.Content = avatar.Content;
                    entity.CreatedTime = DateTime.Now;
                }
                else
                {
                    dbContext.Avatars.Add(avatar.ToEntity());
                }

                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 根据会员ID获取头像
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>头像详细信息</returns>
        public AvatarModel GetByUserId(int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Avatars.FirstOrDefault(item => (item.UserId == userId));
                if (entity != null)
                {
                    return entity.ToModel();
                }
            }

            return null;
        }

    }
}