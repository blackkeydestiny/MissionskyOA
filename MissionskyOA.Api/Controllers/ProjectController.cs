using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;
using MissionskyOA.Web.Kendoui;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 项目管理
    /// </summary>
    [RoutePrefix("api/project")]
    public class ProjectController : BaseController
    {
        private IProjectService ProjectService { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectService"></param>
        public ProjectController(IProjectService projectService)
        {
            this.ProjectService = projectService;
        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <returns></returns>
        [Route("get/all")]
        [HttpGet]
        public ApiListResponse<ProjectModel> GetAllProjects()
        {

            ApiListResponse<ProjectModel> response = new ApiListResponse<ProjectModel>()
            {
                Result = this.ProjectService.SearchAllProject()
            };

            return response;
        }

        /// <summary>
        /// 获取单个项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get/{id}")]
        [HttpGet]
        public ApiResponse<ProjectModel> GetProject(int id)
        {

            ApiResponse<ProjectModel> response = new ApiResponse<ProjectModel>()
            {
                Result = this.ProjectService.SearchProject(id)
            };

            return response;
        }

        /// <summary>
        /// 新增项目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        public ApiResponse<ProjectModel> AddProject(ProjectModel model)
        {
            var project = new ProjectModel()
            {
                ProjectId = model.ProjectId,
                Name = model.Name,
                CreatedTime = DateTime.Now
            };

            ApiResponse<ProjectModel> response = new ApiResponse<ProjectModel>()
            {
                Result = ProjectService.AddProject(project)
            };

            return response;
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("edit/{id}")]
        [HttpPut]
        public ApiResponse<bool> UpdateProject(int id, ProjectModel model)
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
                Result = this.ProjectService.UpdateProject(id, model)
            };
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("delete/{id}")]
        [HttpDelete]
        public ApiResponse<bool> DeleteProject(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = ProjectService.DeleteProject(id)
            };

            return response;
        }
    }
}
