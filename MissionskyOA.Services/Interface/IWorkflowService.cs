using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作流接口
    /// </summary>
    public interface IWorkflowService
    {
        /// <summary>
        /// 查找单个流程
        /// </summary>
        /// <param name="dbContext">数据库</param>
        /// <param name="model">查询条件模型</param>
        /// <returns>单个流程</returns>
        WorkflowModel GetSingleWorkflow(MissionskyOAEntities dbContext, SearchWorkflowModel model);/// <summary>
                                                                                                   /// 
        /// 查找单个流程
        /// </summary>
        /// <param name="model">查询条件模型</param>
        /// <returns>单个流程</returns>
        WorkflowModel GetSingleWorkflow(SearchWorkflowModel model);

        /// <summary>
        /// 根据步骤Id获取详细
        /// </summary>
        /// <param name="steps">流程步骤</param>
        /// <param name="stepId">当前步骤Id</param>
        /// <returns>流程步骤详细</returns>
        WorkflowStepModel GetStepDetailById(IList<WorkflowStepModel> steps, int stepId);


        /// <summary>
        /// 根据步骤Id获取详细
        /// </summary>
        /// <param name="flowId">流程Id</param>
        /// <param name="stepId">当前步骤Id</param>
        /// <returns>流程步骤详细</returns>
        WorkflowStepModel GetStepDetailById(int flowId, int stepId);

        /// <summary>
        /// 根据步骤Id获取详细
        /// </summary>
        /// <param name="stepId">步骤Id</param>
        /// <returns>流程步骤详细</returns>
        WorkflowStepModel GetStepDetailById(int stepId);

        /// <summary>
        /// 查询流程列表
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <returns>流程列表</returns>
        ListResult<WorkflowModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 根据id查询流程详细
        /// </summary>
        /// <param name="flowId">流程Id</param>
        /// <returns>流程详细</returns>
        WorkflowModel GetWorkflowDetail(int flowId);

        /// <summary>
        /// 根据id查询流程详细
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="flowId">流程Id</param>
        /// <returns>流程详细</returns>
        WorkflowModel GetWorkflowDetail(MissionskyOAEntities dbContext, int flowId);

        /// <summary>
        /// 添加工作流
        /// </summary>
        /// <param name="model">工作流信息</param>
        /// <returns>工作流id</returns>
        int AddWorkflow(WorkflowModel model);

        /// <summary>
        /// 更新工作流
        /// </summary>
        /// <param name="model">工作流信息</param>
        /// <returns>true or false</returns>
        bool UpdateWorkflow(WorkflowModel model);

        /// <summary>
        /// 删除工作流
        /// </summary>
        /// <param name="id">工作流id</param>
        /// <returns>是否删除成功</returns>
        bool DeleteWorkflow(int id);

        /// <summary>
        /// 添加工作流流骤
        /// </summary>
        /// <param name="model">工作流步骤</param>
        /// <returns>工作流步骤id</returns>
        int AddWorkflowStep(WorkflowStepModel model);
        
        /// <summary>
        /// 更新工作流步骤
        /// </summary>
        /// <param name="model">工作流步骤</param>
        /// <returns>true or false</returns>
        bool UpdateWorkflowStep(WorkflowStepModel model);
        
        /// <summary>
        /// 删除工作流步骤
        /// </summary>
        /// <param name="id">工作流步骤id</param>
        /// <returns>是否删除成功</returns>
        bool DeleteWorkflowStep(int id);
    }
}
