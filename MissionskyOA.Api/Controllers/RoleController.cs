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

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [RoutePrefix("api/role")]
    public class RoleController : BaseController
    {
        private IRoleService RoleService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleService"></param>
        public RoleController(IRoleService roleService)
        {
            this.RoleService = roleService;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public ApiListResponse<RoleModel> GetAllRoles()
        {
            ApiListResponse<RoleModel> response = new ApiListResponse<RoleModel>()
            {
                Result = this.RoleService.SearchAllRole()
            };

            return response;
        }

        /// <summary>
        /// 获取单个角色
        /// </summary>
        /// <param name="id">查询条件</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ApiResponse<RoleModel> GetRole(int id)
        {
            ApiResponse<RoleModel> response = new ApiResponse<RoleModel>()
            {
                Result = this.RoleService.SearchRole(id)
            };

            return response;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model">新增角色信息</param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ApiResponse<RoleModel> AddRole(RoleModel model)
        {
            var role = new RoleModel()
            {
                Id = model.Id,
                RoleName = model.RoleName,
                ApprovedDays = model.ApprovedDays,
                CreatedTime = DateTime.Now
            };

            ApiResponse<RoleModel> response = new ApiResponse<RoleModel>()
            {
                Result = RoleService.AddRole(role)
            };

            return response;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpPut]
        public ApiResponse<bool> UpdateRole(int id, RoleModel model)
        {
            if (model == null)
            {
                throw new Exception("The request body cant't be null.");
            }

            //验证ID
            if (id < 1)
            {
                throw new Exception("Invalid id.");
            }

            return new ApiResponse<bool>
            {
                Result = this.RoleService.UpdateRole(id, model)
            };
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ApiResponse<bool> DelteRole(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = RoleService.DeleteRole(id)
            };

            return response;
        }
    }
}
