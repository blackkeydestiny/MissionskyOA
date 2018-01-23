using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [RoutePrefix("api/users")]
    public class UserController : BaseController
    {
        private IUserTokenService UserTokenService { get; set; }

        private IAttendanceSummaryService AttendanceSummaryService { get; set; }

        private IUserService UserService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserController(IUserTokenService userTokenService, IAttendanceSummaryService attendanceSummaryService, IUserService userService)
        {
            this.UserTokenService = userTokenService;
            this.AttendanceSummaryService = attendanceSummaryService;
            this.UserService = userService;
        }

        /// <summary>
        /// 根据用户ID 获取员工的基本信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>用户基本信息</returns>
        [Route("{id}")]
        [HttpGet]
        public ApiResponse<UserModel> GetUserDetail(int id)
        {
            if (id < 1)
            {
                throw new Exception("Invalid id.");
            }
            
            var user = this.UserService.GetUserDetail(id);
            var current = UserTokenService.GetMemberByToken(this.Token);
            UserExtentions.HidePhone(current, user); //隐藏电话号码

            ApiResponse<UserModel> response = new ApiResponse<UserModel>()
            {
                Result = user
            };

            return response;
        }
        
        /// <summary>
        /// 根据电话号码/邮箱/中文名/英文名/QQ号获取员工的基本信息
        /// </summary>
        /// <param name="model">用户信息</param>
        /// <returns></returns>
        [Route("getUser")]
        [HttpPost]
        public ApiResponse<UserModel> GetUserDetail(SingleUserModel model)
        {
            if (model == null || (string.IsNullOrEmpty(model.Phone) && string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.ChineseName) 
                && string.IsNullOrEmpty(model.EnglishName) && string.IsNullOrEmpty(model.QQID)))
            {
                throw new Exception("The request body cant't be null.");
            }

            var user = this.UserService.GetUserDetail(model);
            var current = UserTokenService.GetMemberByToken(this.Token);
            UserExtentions.HidePhone(current, user); //隐藏电话号码

            ApiResponse<UserModel> response = new ApiResponse<UserModel>()
            {
                Result = user
            };

            return response;
        }

        /// <summary>
        /// 分页查询用户(员工)
        /// 默认查询所有在职员工
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的用户</returns>
        [Route("search")]
        [HttpPost]
        public ApiPagingListResponse<UserModel> SearchUsers(SearchUserModel model, int pageIndex = 0, int pageSize = 25)
        {
            //查询满足条件的用户
            var query = this.UserService.SearchUsers(model, pageIndex, pageSize);

            //查询结果
            var result = new PaginationModel<UserModel>();
            result.Result = query.Result;

            //分页
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<UserModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 获取领导列表
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<UserModel>> GetLeaders()
        {
            return new ApiResponse<List<UserModel>>
            {
                Result = this.UserService.GetLeaders()
            };
        }

        /// <summary>
        /// 获取部门数据
        /// </summary>
        /// <returns></returns>
        [Route("get/dept")]
        [HttpGet]
        public ApiResponse<List<DepartmentModel>> GetDepartments()
        {
            return new ApiResponse<List<DepartmentModel>>
            {
                Result = this.UserService.GetDepartments()
            };
        }

        /// <summary>
        /// 获取当前用户的详细信息
        /// </summary>
        /// <returns>当前用户详细信息:基本信息，在职信息，假期信息等</returns>
        [Route("getCurrentUser")]
        [HttpGet]
        public ApiResponse<UserModel> GetCurrentUserDetail()
        {
            var member = this.UserTokenService.GetMemberByToken(this.Token);

            return new ApiResponse<UserModel>
            {
                Result = member
            };
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="model">用户基本信息</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        [Route("{id}/update")]
        [HttpPut]
        public ApiResponse<bool> UpdateUser(int id, UpdateUserModel model)
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
                Result = this.UserService.UpdateUser(id, model)
            };
        }

        /// <summary>
        /// 删除用户,注意删除关联数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ApiResponse<bool> DelteUser(int id)
        {
            return new ApiResponse<bool>
            {
                Result = true
            };
        }

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/lock")]
        [HttpGet]
        public ApiResponse<bool> LockUser(int id)
        {
            return new ApiResponse<bool>
            {
                Result = true
            };
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}/roles")]
        [HttpPut]
        public ApiResponse<bool> UpdateRoles(int id, object model)
        {
            return new ApiResponse<bool>
            {
                Result = true
            };
        }

        /// <summary>
        /// 用户消息推送授权
        /// </summary>
        /// <param name="updatedItems">变更的授权项</param>
        /// <returns></returns>
        [Route("auth/notify")]
        [HttpPut]
        public ApiResponse<bool> ChangeNotifyAuth(Dictionary<string, bool> updatedItems)
        {
            return new ApiResponse<bool>
            {
                Result = this.UserService.ChangeNotifyAuth(this.Token, updatedItems)
            };
        }
    }
}
