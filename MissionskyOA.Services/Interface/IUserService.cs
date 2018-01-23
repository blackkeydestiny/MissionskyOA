using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;

namespace MissionskyOA.Services
{
    /// <summary>
    /// User Interface
    /// </summary>
    public partial interface IUserService
    {
        /// <summary>
        /// 根据用户ID 获取员工的基本信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>用户基本信息</returns>
        UserModel GetUserDetail(int id);

        /// <summary>
        /// 根据电话号码/邮箱/中文名/英文名/QQ号获取员工的基本信息
        /// </summary>
        /// <param name="model">用户信息</param>
        /// <returns>用户信息</returns>
        UserModel GetUserDetail(SingleUserModel model);

        /// <summary>
        /// 查询用户(员工)
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <returns>满足条件的用户</returns>
        List<UserModel> GetUserList(SearchUserModel model = null);

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="token">登录用户Token</param>
        /// <param name="model">新旧密码</param>
        /// <returns>是否重置成功</returns>
        bool UpdatePassowrd(string token, UpdatePasswordModel model);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="model">用户信息</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        bool UpdateUser(int id, UpdateUserModel model);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>是否重置成功</returns>
        bool ResetPassword(ResetPasswordModel user);

        /// <summary>
        /// 分页查询用户(员工)
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的用户</returns>
        IPagedList<UserModel> SearchUsers(SearchUserModel model, int pageIndex, int pageSize);

        UserModel Login(LoginUserModel model);

        /// <summary>
        /// 显示用户列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        ListResult<UserModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="model">用户信息</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        bool AddUser(UserModel model);

        /// <summary>
        /// 查询所有管理层(员工)
        /// </summary>
        /// <returns>满足条件的用户</returns>
        List<UserModel> GetManagementUsers();

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="id">删除用户信息</param>
        /// <param name="model">删除用户信息</param>
        /// <returns></returns>
        /// <remarks>是否删除成功</remarks>
        bool Remove(int id);

        List<DepartmentModel> GetDets();

        /// <summary>
        /// 导入用户假期基准信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>假期基准信息</returns>
        NameValueCollection ImportUserBaseVacation(int id);

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        List<UserModel> GetAllUsers();

        /// <summary>
        /// 获取多个用户英文名
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userIds">用户Id</param>
        /// <returns></returns>
        string GetUsersName(MissionskyOAEntities dbContext, int[] userIds);

        /// <summary>
        /// 获取申请单用户信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userIds">申请单用户</param>
        /// <returns></returns>
        IList<OrderUserModel> GetOrderUsers(MissionskyOAEntities dbContext, int[] userIds);

        /// <summary>
        /// 获取申请单用户信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="orderNo">批量申请Id</param>
        /// <returns></returns>
        IList<OrderUserModel> GetOrderUsers(MissionskyOAEntities dbContext, int orderNo);

        /// <summary>
        /// 用户消息推送授权
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updatedItems">变更后的授权项</param>
        /// <returns></returns>
        bool ChangeNotifyAuth(string token, Dictionary<string, bool> updatedItems);

        /// <summary>
        /// 获取领导信息
        /// </summary>
        /// <returns></returns>
        List<UserModel> GetLeaders();

        /// <summary>
        /// 获取部门数据
        /// </summary>
        /// <returns></returns>
        List<DepartmentModel> GetDepartments();
    }
}
