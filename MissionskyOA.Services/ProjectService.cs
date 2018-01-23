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
    public class ProjectService : IProjectService
    {
        /// <summary>
        /// 添加项目信息
        /// </summary>
        /// <param name="project">项目信息</param>
        /// <returns>返回项目成功</returns>
        public ProjectModel AddProject(ProjectModel project)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var projectEntity = project.ToEntity();
                projectEntity.CreatedTime = DateTime.Now;

                var entity = dbContext.Projects.Add(projectEntity);
                dbContext.SaveChanges();
                return projectEntity.ToModel();
            }
        }

        /// <summary>
        /// 更新项目名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="project">更新的信息</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateProject(int id,ProjectModel project)
        {
            using (var dbContext = new MissionskyOAEntities())
            {

                var entity = dbContext.Projects.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("cannot find project.");
                }
                entity.Name = project.Name;
                entity.ProjectManager = project.ProjectManager;
                entity.ProjectBegin=project.ProjectBegin;
                entity.ProjectEnd=project.ProjectEnd;
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 查询项目信息
        /// </summary>
        /// <returns>查询结果</returns>
        public List<ProjectModel> SearchAllProject()
        {
            using (var dbContext = new MissionskyOAEntities())
            {

                var projects = dbContext.Projects.Where(it => it.Id != null);

                List<ProjectModel> result = new List<ProjectModel>();

                projects.ToList().ForEach(entity =>
                {
                    result.Add(entity.ToModel());
                });

                return result;
            }

        }

        /// <summary>
        /// 查询项目信息
        /// </summary>
        /// <param name="id">查询条件</param>
        /// <returns>查询结果</returns>
        public ProjectModel SearchProject(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Projects.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid Project Id.");
                }
                var model = entity.ToModel();
                return model;
            }
        }

        /// <summary>
        /// 删除项目信息
        /// </summary>
        /// <param name="id">删除条件</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteProject(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Projects.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    throw new KeyNotFoundException("无效的ID");
                }

                var userInProject = dbContext.Users.Where(it => it.ProjectId == id);
                userInProject.ToList().ForEach(it =>
                {
                    it.ProjectId = null;
                });

                entity.Status = 0;
                dbContext.SaveChanges();
                return true;
            }
        }

        public ListResult<ProjectModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Projects.AsEnumerable();

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

                ListResult<ProjectModel> result = new ListResult<ProjectModel>();
                result.Data = new List<ProjectModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }

        /// <summary>
        /// 查询项目组关联用户信息
        /// </summary>
        /// <returns>查询结果</returns>
        public ListResult<UserModel> RelatedProjectUser(int projectId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var relatedUser = dbContext.Users.Where(item => item.ProjectId == projectId);
                ListResult<UserModel> result = new ListResult<UserModel>();
                result.Data = new List<UserModel>();
                relatedUser.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });
                result.Total = relatedUser.Count();
                return result;
            }
        }

        /// <summary>
        /// 解除用户与项目组绑定
        /// </summary>
        /// <returns>查结果</returns>
        public bool UnRelatedUserProject(int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.FirstOrDefault(it => it.Id == userId);
                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid User Id.");
                }
                entity.ProjectId = null;
                dbContext.SaveChanges();
                return true;
            }
        }
    }
}