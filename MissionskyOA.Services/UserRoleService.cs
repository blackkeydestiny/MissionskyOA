using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Email;
using MissionskyOA.Core.Security;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using MissionskyOA.Services.Extentions;

namespace MissionskyOA.Services
{
    public class UserRoleService : IUserRoleService
    {
        /// <summary>
        /// 添加用户角色
        /// </summary>
        /// <param name="userrole"></param>
        /// <returns>是否添加成功</returns>
        public UserRolesModel AddUserRole(UserRolesModel userrole)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var userroleEntity = userrole.ToEntity();
                dbContext.UserRoles.Add(userroleEntity);
                dbContext.SaveChanges();
                return userroleEntity.ToModel();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userrole"></param>
        /// <returns>是否删除成功</returns>
        public bool DeleteUserRole(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.UserRoles.Where(it => it.UserId == id).FirstOrDefault();

                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid id.");
                }

                dbContext.UserRoles.Remove(entity);
                dbContext.SaveChanges();
                return true;
            }
        }
    }
}