using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// Role Interface
    /// </summary>
    public partial interface IRoleService
    {
        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="model">角色信息</param>
        /// <returns>是否重置成功</returns>
        RoleModel AddRole(RoleModel model);

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model">角色更新的信息</param>
        /// <returns>是否更新成功</returns>
        bool UpdateRole(int id,RoleModel model);

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns>查询结果</returns>
        RoleModel SearchRole(int id);

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <returns>查询结果</returns>
        List<RoleModel> SearchAllRole();

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="id">删除条件</param>
        /// <returns>是否删除成功</returns>
        bool DeleteRole(int id);
        
        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <returns>查询结果</returns>
        ListResult<RoleModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);
        
        /// <summary>
        /// 查询角色关联用户信息
        /// </summary>
        /// <returns>查询结果</returns>
        ListResult<UserRolesModel> RelatedUser(int roleId);


    }
}
