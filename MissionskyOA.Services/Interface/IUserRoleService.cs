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
    public partial interface IUserRoleService
    {
        /// <summary>
        /// 添加用户角色关联信息
        /// </summary>
        /// <param name="model">信息</param>
        /// <returns>是否重置成功</returns>
        UserRolesModel AddUserRole(UserRolesModel model);

        /// <summary>
        /// 删除关联用户信息
        /// </summary>
        /// <returns>查询结果</returns>
        bool DeleteUserRole(int id);

    }
}
