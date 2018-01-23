using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MissionskyOA.Services.Interface;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [RoutePrefix("api/userrole")]
    public class UserRoleController : BaseController
    {
        private IUserRoleService UserRoleService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userroleService"></param>
        public UserRoleController(IUserRoleService userroleService)
        {
            this.UserRoleService = userroleService;
        }


        /// <summary>
        /// 新增员工角色关联关系
        /// </summary>
        /// <param name="model">新增角色信息</param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ApiResponse<UserRolesModel> AddUserRole(UserRolesModel model)
        {
            var userrole = new UserRolesModel()
            {
                UserRoleId = model.UserRoleId,
                UserId = model.UserId,
                RoleId = model.RoleId,
            };

            ApiResponse<UserRolesModel> response = new ApiResponse<UserRolesModel>()
            {
                Result = UserRoleService.AddUserRole(userrole)
            };

            return response;
        }

        
    }
}
