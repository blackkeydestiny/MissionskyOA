using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using log4net;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Portal.Controllers
{
    public class WorkflowController : Controller
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(WorkflowController));

        private IWorkflowService WorkflowService;
        private IUserService UserService;
        private IRoleService RoleService;
        //
        // GET: /Area/

        public WorkflowController(IWorkflowService workflowService, IUserService userService, IRoleService roleService)
        {
            this.WorkflowService = workflowService;
            this.UserService = userService;
            this.RoleService = roleService;
        }

        //
        // GET: /Workflow/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Workflow/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue || id.Value < 1)
            {
                throw new KeyNotFoundException("Invalid workflow id.");
            }

            var model = WorkflowService.GetWorkflowDetail(id.Value);
            return View(model);
        }

        //
        // GET: /Workflow/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Workflow/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Workflow/Edit/5
        public ActionResult Edit(int? id)
        {
            var selectedType = WorkflowType.None.ToString();

            WorkflowModel model = null;

            if (!id.HasValue || id.Value < 1) //新增
            {
                model = new WorkflowModel()
                {
                    Status = WorkflowStatus.Enabled,
                    WorkflowSteps = new List<WorkflowStepModel>()
                };
            }
            else //修改
            {
                model = WorkflowService.GetWorkflowDetail(id.Value);
                
                //流程类型
                selectedType = model.Type == WorkflowType.None? WorkflowType.AskLeave.ToString() :  model.Type.ToString();
            }

            ViewData["WorkflowStep"] = model.WorkflowSteps;
            InitWorkflowTypes(selectedType);
            ViewData["WorkflowId"] = model.Id;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(WorkflowModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入工作名称.");
            }

            if (model.Type == WorkflowType.None)
            {
                ModelState.AddModelError("Type", "请选择工作流类型.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        this.WorkflowService.UpdateWorkflow(model);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var workflowId = this.WorkflowService.AddWorkflow(model);
                        return RedirectToAction("Edit", new { id = workflowId });
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ViewBag.Message = ex.Message;
                }
            }

            InitWorkflowTypes(model.Type.ToString());
            ViewData["WorkflowStep"] = model.WorkflowSteps ?? new List<WorkflowStepModel>();

            return View(model);
        }

        /// <summary>
        /// 删除报表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                WorkflowService.DeleteWorkflow(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { error = ex.Message });
            }
        }

        //
        // GET: /Workflow/Edit/5
        public ActionResult EditStep(int flowId, int? id)
        {
            if (flowId < 1)
            {
                return RedirectToAction("Edit");
            }

            #region 获取流程步骤类型

            NameValueCollection collection = EnumExtensions.GetDescriptionList(typeof (WorkflowStepType));
            var stepTypes = new List<SelectListItem>();
            var selectedStepType = (int) WorkflowStepType.None;
            stepTypes.Add(new SelectListItem() {Value = "0", Text = "请选择流程步骤类型"});

            foreach (string key in collection)
            {
                if (key != WorkflowStepType.None.ToString())
                {
                    stepTypes.Add(new SelectListItem()
                    {
                        Value = ((int) Enum.Parse(typeof (WorkflowStepType), key)).ToString(),
                        Text = collection[key]
                    });
                }
            }

            #endregion

            #region 获取流程步骤类型
            collection = EnumExtensions.GetDescriptionList(typeof (WorkflowOperator));
            var operatorTypes = new List<SelectListItem>();
            var selectedOperatorTypes = (int) WorkflowOperator.None;

            foreach (string key in collection)
            {
                if (key != WorkflowOperator.None.ToString())
                {
                    operatorTypes.Add(new SelectListItem()
                    {
                        Value = ((int) Enum.Parse(typeof (WorkflowOperator), key)).ToString(),
                        Text = collection[key]
                    });
                }
            }

            #endregion

            #region 用户角色

            var roleList = this.RoleService.SearchAllRole();
            roleList = roleList ?? new List<RoleModel>();

            var roles = new List<SelectListItem>();
            roles.Add(new SelectListItem() {Value = "0", Text = "请选择用户角色"});

            var selectedRole = 0;

            roleList.ForEach(it => roles.Add(new SelectListItem() {Value = it.Id.ToString(), Text = it.RoleName}));

            #endregion

            #region 审批用户
            var selectedUser = 0;
            var userList = this.UserService.GetUserList(new SearchUserModel() {Status = AccountStatus.Normal});
            userList = userList ?? new List<UserModel>();

            var users = new List<SelectListItem>();
            users.Add(new SelectListItem() {Value = "0", Text = "请选择用户"});

            //工程师角色
            var eng =
                roleList.FirstOrDefault(
                    it =>
                        it.RoleName == "工程师" ||
                        (!string.IsNullOrEmpty(it.Abbreviation) && it.Abbreviation.ToLower().Equals("eng")));
            eng = eng ?? new RoleModel();

            userList = userList.Where(it => it.Role != 0 && it.Role != eng.Id).ToList(); //不为工程师的其它用户
            userList.ForEach(it =>
            {
                users.Add(new SelectListItem()
                {
                    Value = it.Id.ToString(),
                    Text = string.Format("{0}({1})", it.EnglishName, it.ChineseName)
                });
            });

            #endregion

            #region 流程

            var workflow = WorkflowService.GetWorkflowDetail(flowId);
            var steps = new List<SelectListItem>();
            steps.Add(new SelectListItem() { Value = "0", Text = "请选择流程上一步" });
            var prevStep = 0; //上一步

            if (workflow == null)
            {
                throw new KeyNotFoundException("找不到工作流。");
            }

            foreach (var step in workflow.WorkflowSteps)
            {
                steps.Add(new SelectListItem()
                {
                    Value = step.Id.ToString(),
                    Text = step.Name
                });
            }

            #endregion

            WorkflowStepModel model = null;

            if (!id.HasValue || id.Value < 1) //新增
            {
                model = new WorkflowStepModel()
                {
                    Type = WorkflowStepType.LeaderApprove,
                    OperatorType = WorkflowOperator.User
                };
            }
            else //修改
            {
                model = WorkflowService.GetStepDetailById(id.Value);

                //流程类型
                selectedStepType = model.Type == WorkflowStepType.None
                    ? (int) WorkflowStepType.LeaderApprove
                    : (int) model.Type;

                //用户角色
                selectedRole = model.OperatorType == WorkflowOperator.Role ? model.Operator : 0;

                //用户
                selectedUser = model.OperatorType == WorkflowOperator.User ? model.Operator : 0;

                //审批人类型
                selectedOperatorTypes = model.OperatorType == WorkflowOperator.None
                    ? (int) WorkflowOperator.User
                    : (int) model.OperatorType;

                //上一步
                var prevStepModel = workflow.WorkflowSteps.FirstOrDefault(it => it.NextStep == id);
                if (prevStepModel != null)
                {
                    prevStep = prevStepModel.Id;
                }
            }

            ViewData["WorkflowSteps"] = new SelectList(steps, "Value", "Text", prevStep);
            ViewData["WorkflowStepType"] = new SelectList(stepTypes, "Value", "Text", selectedStepType);
            ViewData["WorkflowOperatorType"] = new SelectList(operatorTypes, "Value", "Text", selectedOperatorTypes);
            ViewData["Roles"] = new SelectList(roles, "Value", "Text", selectedRole);
            ViewData["Users"] = new SelectList(users, "Value", "Text", selectedUser);
            InitWorkflow(flowId);
            //ViewData["Users"] = new SelectList(users, "Value", "Text", selectedUser);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditStep(WorkflowStepModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入流程步骤名称.");
            }

            if (model.Type == WorkflowStepType.None)
            {
                ModelState.AddModelError("Type", "请选择流程步骤类型.");
            }

            if (model.Type == WorkflowStepType.LeaderApprove && model.MaxTimes <= 0 && model.MinTimes <= 0)
            {
                ModelState.AddModelError("Type", "请领导审批的时间范围.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id <= 0)
                    {
                        WorkflowService.AddWorkflowStep(model);
                    }
                    else
                    {
                        WorkflowService.UpdateWorkflowStep(model);
                    }

                    return RedirectToAction("Edit", new { id = model.FlowId });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ViewBag.Message = ex.Message;
                }
            }

            EditStep(model.FlowId, model.Id);
            return View(model);
        }

        /// <summary>
        /// 删除流程步骤
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteStep(int id)
        {
            try
            {
                WorkflowService.DeleteWorkflowStep(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="dRequest">数据请求</param>
        /// <returns></returns>
        public ActionResult Read([DataSourceRequest]DataSourceRequest dRequest)
        {
            SortModel sort = null;
            if (dRequest.Sorts != null && dRequest.Sorts.Count > 0)
            {
                sort = dRequest.Sorts[0].ToSortModel();
            }

            FilterModel filter = null;
            if (dRequest.Filters != null && dRequest.Filters.Count > 0)
            {
                filter = ((FilterDescriptor)dRequest.Filters[0]).ToSortModel();
            }

            var areaResult = WorkflowService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
            };

            return Json(result);
        }

        /// <summary>
        /// 设置工作流类型
        /// </summary>
        /// <param name="selectedType">选择工作流类型</param>
        private void InitWorkflowTypes(string selectedType)
        {
            NameValueCollection collection = EnumExtensions.GetDescriptionList(typeof(WorkflowType));
            var types = new List<SelectListItem>();

            foreach (string key in collection)
            {
                if (key == WorkflowType.None.ToString())
                {
                    types.Add(new SelectListItem() { Value = key, Text = "请选择流程类型" });
                }
                else
                {
                    types.Add(new SelectListItem() { Value = key, Text = collection[key] });
                }
            }

            ViewData["WorkflowType"] = new SelectList(types, "Value", "Text", selectedType);
        }

        /// <summary>
        /// 初始化流程
        /// </summary>
        /// <param name="flowId"></param>
        private void InitWorkflow(int flowId)
        {
            ViewData["Workflow"] = WorkflowService.GetWorkflowDetail(flowId);
        }
    }
}
