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
    public partial interface IProjectService
    {
        /// <summary>
        /// 添加项目信息
        /// </summary>
        /// <param name="model">项目信息</param>
        /// <returns>是否增加成功</returns>
        ProjectModel AddProject(ProjectModel model);

        /// <summary>
        /// 更新项目关联的员工
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model">更新的信息</param>
        /// <returns>是否更新成功</returns>
        bool UpdateProject(int id,ProjectModel model);

        /// <summary>
        /// 查询项目信息
        /// </summary>
        /// <returns>查询结果</returns>
        List<ProjectModel> SearchAllProject();
        /// <summary>
        /// 查询项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns>查询结果</returns>
        ProjectModel SearchProject(int id);

        /// <summary>
        /// 删除项目信息
        /// </summary>
        /// <returns>是否删除成功</returns>
        bool DeleteProject(int id);

        /// <summary>
        /// 项目信息列表
        /// </summary>
        /// <returns>项目信息列表</returns>
        ListResult<ProjectModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

         /// <summary>
        /// 查询项目组关联用户信息
        /// </summary>
        /// <returns>查询结果</returns>
        ListResult<UserModel> RelatedProjectUser(int projectId);

         /// <summary>
        /// 移除此用户与项目组绑定
        /// </summary>
        /// <returns>查询结果</returns>
        bool UnRelatedUserProject(int userId);
    }
}
