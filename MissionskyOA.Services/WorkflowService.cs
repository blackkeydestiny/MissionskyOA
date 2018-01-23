using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作流类
    /// </summary>
    public class WorkflowService : ServiceBase, IWorkflowService
    {
        /// <summary>
        /// 查找单个流程
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="model">查询条件模型</param>
        /// <returns>单个流程</returns>
        public WorkflowModel GetSingleWorkflow(MissionskyOAEntities dbContext, SearchWorkflowModel model)
        {
            if (dbContext == null)
            {
                throw new NullReferenceException("Invalid db context for get single workflow.");
            }

            if (model == null)
            {
                throw new NullReferenceException("Invalid workflow search model for get single workflow.");
            }

            //1. 默认查询所有状态的流程 || 查询启用或禁用的流程
            var query =
                dbContext.Workflows.Where(
                    it => model.Status == WorkflowStatus.None || it.Status == (model.Status == WorkflowStatus.Enabled));

            //2. 查询所有的流程 || 指定流程名的流程
            query = query.Where(it => string.IsNullOrEmpty(model.Name) || it.Name.Contains(model.Name));

            //3. 查询所有的流程 || 指定流程ID的流程
            query = query.Where(it => model.WorkflowId < 1 || it.Id == model.WorkflowId);

            //4. 查询所有的流程 || 指定流程类型的流程
            query = query.Where(it => model.Type == WorkflowType.None || it.Type == (int) model.Type);

            //5. 查询单个流程
            var entity = query.FirstOrDefault();

            if (entity == null)
            {
                throw new KeyNotFoundException("Invalid query");
            }

            if (entity.WorkflowSteps == null)
            {
                var steps = dbContext.WorkflowSteps.Where(it => it.FlowId == entity.Id).OrderBy(it => it.Id);

                if (steps != null)
                {
                    entity.WorkflowSteps = steps.ToList();
                }
            }

            return entity.ToModel();
        }

        /// <summary>
        /// 查找单个流程
        /// </summary>
        /// <param name="model">查询条件模型</param>
        /// <returns>单个流程</returns>
        public WorkflowModel GetSingleWorkflow(SearchWorkflowModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Invalid workflow search model for get single workflow.");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //1. 默认查询所有状态的流程 || 查询启用或禁用的流程
                var query =
                    dbContext.Workflows.Where(
                        it =>
                            model.Status == WorkflowStatus.None || it.Status == (model.Status == WorkflowStatus.Enabled));

                //2. 查询所有的流程 || 指定流程名的流程
                query = query.Where(it => string.IsNullOrEmpty(model.Name) || it.Name.Contains(model.Name));

                //3. 查询所有的流程 || 指定流程ID的流程
                query = query.Where(it => model.WorkflowId < 1 || it.Id == model.WorkflowId);

                //4. 查询所有的流程 || 指定流程类型的流程
                query = query.Where(it => model.Type == WorkflowType.None || it.Type == (int) model.Type);

                //5. 查询单个流程
                var entity = query.FirstOrDefault();

                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid query");
                }

                if (entity.WorkflowSteps == null)
                {
                    var steps = dbContext.WorkflowSteps.Where(it => it.FlowId == entity.Id).OrderBy(it => it.Id);

                    if (steps != null)
                    {
                        entity.WorkflowSteps = steps.ToList();
                    }
                }

                return entity.ToModel();
            }
        }

        /// <summary>
        /// 根据id查询流程详细
        /// </summary>
        /// <param name="flowId">流程Id</param>
        /// <returns>流程详细</returns>
        public WorkflowModel GetWorkflowDetail(int flowId)
        {
            if (flowId < 0)
            {
                throw new KeyNotFoundException("Invalid workflow id for query detail.");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Workflows.Where(it => it.Id == flowId).FirstOrDefault();

                if (entity == null)
                {
                    throw new KeyNotFoundException("Cannot query the workflow detail by id.");
                }

                if (entity.WorkflowSteps == null)
                {
                    var steps = dbContext.WorkflowSteps.Where(it => it.FlowId == entity.Id).OrderBy(it => it.Id);
                    entity.WorkflowSteps = steps.ToList();
                }

                return entity.ToModel();
            }
        }

        /// <summary>
        /// 根据id查询流程详细
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="flowId">流程Id</param>
        /// <returns>流程详细</returns>
        public WorkflowModel GetWorkflowDetail(MissionskyOAEntities dbContext, int flowId)
        {
            if (flowId < 0)
            {
                Log.Error("流程无效。");
                throw new KeyNotFoundException("Invalid workflow id for query detail.");
            }

            var entity = dbContext.Workflows.Where(it => it.Id == flowId).FirstOrDefault();

            if (entity == null)
            {
                Log.Error("未找到相关流程。");
                throw new KeyNotFoundException("Cannot query the workflow detail by id.");
            }

            if (entity.WorkflowSteps == null)
            {
                var steps = dbContext.WorkflowSteps.Where(it => it.FlowId == entity.Id).OrderBy(it => it.Id);
                entity.WorkflowSteps = steps.ToList();
            }

            return entity.ToModel();
        }

        /// <summary>
        /// 根据步骤Id获取详细
        /// </summary>
        /// <param name="steps">流程步骤</param>
        /// <param name="stepId">当前步骤Id</param>
        /// <returns>流程步骤详细</returns>
        public WorkflowStepModel GetStepDetailById(IList<WorkflowStepModel> steps, int stepId)
        {
            if (steps == null || steps.Count < 1 || stepId < 1)
            {
                throw new KeyNotFoundException("Invalid workflow");
            }

            var detail = steps.FirstOrDefault(it => it.Id == stepId);

            if (detail == null) //找不到当前步骤
            {
                throw new KeyNotFoundException("Invalid workflow");
            }

            return detail;
        }

        /// <summary>
        /// 根据步骤Id获取详细
        /// </summary>
        /// <param name="flowId">流程Id</param>
        /// <param name="stepId">当前步骤Id</param>
        /// <returns>流程步骤详细</returns>
        public WorkflowStepModel GetStepDetailById(int flowId, int stepId)
        {
            if (flowId < 1 || stepId < 1)
            {
                throw new KeyNotFoundException("Invalid workflow");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var step = dbContext.WorkflowSteps.FirstOrDefault(it => it.FlowId == flowId && it.Id == stepId); //获取流程步取

                if (step == null)
                {
                    throw new KeyNotFoundException("Invalid workflow");
                }

                return step.ToModel();
            }
        }

        /// <summary>
        /// 根据步骤Id获取详细
        /// </summary>
        /// <param name="stepId">步骤Id</param>
        /// <returns>流程步骤详细</returns>
        public WorkflowStepModel GetStepDetailById(int stepId)
        {
            if (stepId < 1)
            {
                throw new KeyNotFoundException("Invalid workflow");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var step = dbContext.WorkflowSteps.FirstOrDefault(it => it.Id == stepId); //获取流程步取

                if (step == null)
                {
                    Log.Error(string.Format("无效的工作流步骤。步骤id: {0}", stepId));
                    throw new KeyNotFoundException(string.Format("无效的工作流步骤。步骤id: {0}", stepId));
                }

                var workflow = dbContext.Workflows.FirstOrDefault(it => it.Id == step.FlowId);
                if (workflow == null)
                {
                    Log.Error(string.Format("无效的工作流。流程id: {0}", step.FlowId));
                    throw new KeyNotFoundException(string.Format("无效的工作流。流程id: {0}", step.FlowId));
                }

                var model = step.ToModel();
                model.FlowName = workflow.Name;

                return step.ToModel();
            }
        }

        /// <summary>
        /// 查询流程列表
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <returns>流程列表</returns>
        public ListResult<WorkflowModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Workflows.AsEnumerable();

                //查询
                if (filter != null && filter.Member == "Name")
                {
                    switch (filter.Operator)
                    {
                        case "Contains":
                            list = list.Where(it => it.Name != null && it.Name.ToLower().Contains(filter.Value.ToLower()));
                            break;
                    }
                }

                //排序
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

                //分页
                var count = list.Count();
                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                //转换
                ListResult<WorkflowModel> result = new ListResult<WorkflowModel>();
                result.Data = new List<WorkflowModel>();

                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;

                return result;
            }
        }

        /// <summary>
        /// 添加工作流
        /// </summary>
        /// <param name="model">工作流信息</param>
        /// <returns>工作流id</returns>
        public int AddWorkflow(WorkflowModel model)
        {
            if (model == null)
            {
                Log.Error("工作流无效。");
                throw new InvalidOperationException("工作流无效。");
            }
            
            using (var dbContext = new MissionskyOAEntities())
            {
                ValidWorkflow(dbContext, model);

                //添加工作流
                var entity = model.ToEntity();
                entity.CreatedTime = DateTime.Now;
                dbContext.Workflows.Add(entity);
                dbContext.SaveChanges();

                return entity.Id;
            }
        }

        /// <summary>
        /// 更新工作流
        /// </summary>
        /// <param name="model">工作流信息</param>
        /// <returns>true or false</returns>
        public bool UpdateWorkflow(WorkflowModel model)
        {
            if (model == null)
            {
                Log.Error("工作流无效。");
                throw new InvalidOperationException("工作流无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidWorkflow(dbContext, model);

                var oldFlow = dbContext.Workflows.FirstOrDefault(it => it.Id == model.Id);
                if (oldFlow == null)
                {
                    Log.Error(string.Format("找不到工作流, Id: {0}", model.Id));
                    throw new InvalidOperationException(string.Format("找不到工作流, Id: {0}", model.Id));
                }

                //更新工作流
                if (oldFlow.Name == null || !oldFlow.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldFlow.Name = model.Name;
                }

                if (oldFlow.Desc == null || !oldFlow.Desc.Equals(model.Desc, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldFlow.Desc = model.Desc;
                }

                if (oldFlow.Type != Convert.ToInt32(model.Type))
                {
                    oldFlow.Type = (int) model.Type;
                }

                if (oldFlow.Status != Convert.ToBoolean(model.Status))
                {
                    oldFlow.Status = Convert.ToBoolean((int)model.Status);
                }

                if (oldFlow.Condition == null || !oldFlow.Condition.Equals(model.Condition, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldFlow.Condition = model.Condition;
                }

                if (oldFlow.ConditionDesc == null || !oldFlow.ConditionDesc.Equals(model.ConditionDesc, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldFlow.ConditionDesc = model.ConditionDesc;
                }

                //更新工作流
                dbContext.SaveChanges();

                return true;
            }
        }
        
        /// <summary>
        /// 删除工作流
        /// </summary>
        /// <param name="id">工作流id</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteWorkflow(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var isUsed = dbcontext.Orders.Any(it => it.WorkflowId == id);

                if (isUsed)
                {
                    Log.Error("流程已被使用，不能删除。");
                    throw new KeyNotFoundException("流程已被使用，不能删除。");
                }

                var entity = dbcontext.Workflows.FirstOrDefault(it => it.Id == id);

                if (entity == null)
                {
                    Log.Error("找不到工作流。");
                    throw new KeyNotFoundException("找不到工作流。");
                }

                //删除流程步骤
                if (entity.WorkflowSteps != null)
                {
                    entity.WorkflowSteps.ToList().ForEach(it => dbcontext.WorkflowSteps.Remove(it));
                }

                dbcontext.Workflows.Remove(entity);
                dbcontext.SaveChanges();

                return true;
            }
        }
        
        /// <summary>
        /// 添加工作流流骤
        /// </summary>
        /// <param name="model">工作流步骤</param>
        /// <returns>工作流步骤id</returns>
        public int AddWorkflowStep(WorkflowStepModel model)
        {
            if (model == null)
            {
                Log.Error("工作流步骤无效。");
                throw new InvalidOperationException("工作流步骤无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidWorkflowStep(dbContext, model);

                //添加工作流
                var entity = model.ToEntity();
                entity.CreatedTime = DateTime.Now;

                //不是领导审批，不需要设置时长
                if (model.Type != WorkflowStepType.LeaderApprove)
                {
                    entity.MinTimes = 0.0;
                    entity.MaxTimes = 0.0;
                }

                dbContext.WorkflowSteps.Add(entity);
                dbContext.SaveChanges();

                //更新流程步骤下一步
                model.Id = entity.Id;
                UpdateNextStep(dbContext, model);

                return model.Id;
            }
        }

        /// <summary>
        /// 更新工作流步骤
        /// </summary>
        /// <param name="model">工作流步骤</param>
        /// <returns>true or false</returns>
        public bool UpdateWorkflowStep(WorkflowStepModel model)
        {
            if (model == null)
            {
                Log.Error("工作流步骤无效。");
                throw new InvalidOperationException("工作流步骤无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidWorkflowStep(dbContext, model);

                var oldStep = dbContext.WorkflowSteps.FirstOrDefault(it => it.Id == model.Id && it.FlowId == model.FlowId);
                if (oldStep == null)
                {
                    Log.Error(string.Format("找不到工作流步骤, Id: {0}", model.Id));
                    throw new InvalidOperationException(string.Format("找不到工作流步骤, Id: {0}", model.Id));
                }

                //更新工作流步骤
                if (oldStep.Name == null || !oldStep.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldStep.Name = model.Name;
                }

                if (oldStep.Desc == null || !oldStep.Desc.Equals(model.Desc, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldStep.Desc = model.Desc;
                }

                if (oldStep.Type != Convert.ToInt32(model.Type))
                {
                    oldStep.Type = (int)model.Type;
                }

                //更新审批条件
                if (model.Type == WorkflowStepType.LeaderApprove)
                {
                    if (oldStep.MinTimes - model.MinTimes >= 0.000000)
                    {
                        oldStep.MinTimes = model.MinTimes;
                    }

                    if (oldStep.MaxTimes - model.MaxTimes >= 0.000000)
                    {
                        oldStep.MaxTimes = model.MaxTimes;
                    }
                }

                if (oldStep.OperatorType != Convert.ToInt32(model.OperatorType))
                {
                    oldStep.OperatorType = (int)model.OperatorType;
                }

                if (oldStep.Operator != model.Operator)
                {
                    oldStep.Operator = model.Operator;
                }

                #region 审批用户变化，更新Order表
                if ((oldStep.OperatorType == (int) WorkflowOperator.Role && model.OperatorType == WorkflowOperator.User) || //角色审批变为用户审批
                    (model.OperatorType == WorkflowOperator.User &&
                     oldStep.OperatorType != Convert.ToInt32(model.OperatorType) && oldStep.Operator != model.Operator)) //用户审批，审批用户变化
                {
                    dbContext.Orders.Where(it => it.NextStep.HasValue && it.NextStep.Value == model.Id)
                        .ToList()
                        .ForEach(it => it.NextAudit = model.Operator);
                }
                #endregion

                UpdateNextStep(dbContext, model, false); //更新下一步

                //更新工作流
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 删除工作流步骤
        /// </summary>
        /// <param name="id">工作流步骤id</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteWorkflowStep(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var isUsed = dbcontext.Orders.Any(it => it.NextStep.HasValue && it.NextStep.Value == id);

                if (isUsed)
                {
                    Log.Error("已被使用，不能删除步骤。");
                    throw new KeyNotFoundException("已被使用，不能删除步骤。");
                }

                var entity = dbcontext.WorkflowSteps.FirstOrDefault(it => it.Id == id);

                if (entity == null)
                {
                    Log.Error("找不到工作流步骤。");
                    throw new KeyNotFoundException("找不到工作流步骤。");
                }
                
                dbcontext.WorkflowSteps.Remove(entity);
                dbcontext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 更新流程步骤下一步
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="nextStep"></param>
        /// <param name="isSaved"></param>
        private void UpdateNextStep(MissionskyOAEntities dbContext, WorkflowStepModel nextStep, bool isSaved = true)
        {
            if (nextStep.PrevStep.HasValue && nextStep.PrevStep.Value > 0)
            {
                var prevStep = dbContext.WorkflowSteps.FirstOrDefault(
                    it => it.FlowId == nextStep.FlowId && it.Id == nextStep.PrevStep.Value);

                if (prevStep == null)
                {
                    Log.Error("流程步骤上一步不存在。");
                    throw new KeyNotFoundException("流程步骤上一步不存在。");
                }

                if (prevStep.NextStep.HasValue && prevStep.NextStep.Value > 0 && prevStep.NextStep.Value != nextStep.Id)
                {
                    Log.Error("流程步骤上一步不存在。");
                    throw new KeyNotFoundException("流程步骤上一步不存在。");
                }

                prevStep.NextStep = nextStep.Id;

                if (isSaved)
                {
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 验证工作流信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        private void ValidWorkflow(MissionskyOAEntities dbContext, WorkflowModel model)
        {
            var existedFlow =
                dbContext.Workflows.FirstOrDefault(
                    it =>
                        !string.IsNullOrEmpty(it.Name) &&
                        it.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) && model.Id != it.Id);

            if (existedFlow != null)
            {
                Log.Error(string.Format("工作流已经存在，工作流: {0}", model.Name));
                throw new InvalidOperationException(string.Format("工作流已经存在，工作流: {0}", model.Name));
            }
        }


        /// <summary>
        /// 验证工作流信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        private void ValidWorkflowStep(MissionskyOAEntities dbContext, WorkflowStepModel model)
        {
            var existedStep =
                dbContext.WorkflowSteps.FirstOrDefault(
                    it =>
                        !string.IsNullOrEmpty(it.Name) &&
                        it.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) && model.FlowId == it.FlowId && model.Id != it.Id);

            if (existedStep != null)
            {
                Log.Error(string.Format("工作流步骤已经存在，工作流步骤: {0}", model.Name));
                throw new InvalidOperationException(string.Format("工作流步骤已经存在，工作流步骤: {0}", model.Name));
            }
        }
    }
}
