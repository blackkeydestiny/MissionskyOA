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
    public class RoleService : ServiceBase, IRoleService
    {
        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="role">角色信息</param>
        /// <returns>返回角色信息</returns>
        public RoleModel AddRole(RoleModel role)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var roleEntity = role.ToEntity();
                dbContext.Roles.Add(roleEntity);
                dbContext.SaveChanges();
                return roleEntity.ToModel();
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="role">角色更新的信息</param>
        /// <param name="id"></param>
        /// <returns>是否更新成功</returns>
        public bool UpdateRole(int id,RoleModel role)
        {
            using (var dbContext = new MissionskyOAEntities())
            {

                var entity = dbContext.Roles.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    Log.Error(string.Format("cannot find role. Role id: {0}", id));
                    throw new KeyNotFoundException("cannot find role.");
                }

                entity.Name = role.RoleName;
                entity.Status = role.Status;
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <returns>查询结果</returns>
        public List<RoleModel> SearchAllRole()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var roles = dbContext.Roles.Where(it => it.Id > 0&&it.Status!=0);

                List<RoleModel> result = new List<RoleModel>();

                roles.ToList().ForEach(entity =>
                {
                    result.Add(entity.ToModel());
                });

                return result;
            }
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="id">删除信息</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteRole(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Roles.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid Project Id.");
                }
                dbContext.Roles.Remove(entity);
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 查询所有角色信息
        /// </summary>
        ///<param name="id">查询条件</param>
        /// <returns>查询结果</returns>
        public RoleModel SearchRole(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Roles.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    Log.Error(string.Format("查找不到相关角色. 角色Id: {0}", id));
                    throw new KeyNotFoundException("Invalid Role Id.");
                }
                var model = entity.ToModel();
                return model;
            }
        }
        public ListResult<RoleModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Roles.AsEnumerable();

                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "CreatedTime":
                            if (sort.Direction == SortDirection.Ascending)
                            {
                                list = list.OrderBy(item => item.CreatedTime);
                            }
                            else
                            {
                                list = list.OrderByDescending(item => item.CreatedTime);
                            }
                            break;
                        default:
                            break;
                    }
                }

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<RoleModel> result = new ListResult<RoleModel>();
                result.Data = new List<RoleModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }

        public ListResult<UserRolesModel> RelatedUser(int roleId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var relatedUser = dbContext.UserRoles.Where(item => item.RoleId == roleId);
                if (relatedUser == null)
                {
                    throw new Exception("无效Id");
                }
                ListResult<UserRolesModel> result = new ListResult<UserRolesModel>();
                result.Data = new List<UserRolesModel>();
                relatedUser.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });
                result.Total = relatedUser.Count();
                return result;
            }

        }

    }
}