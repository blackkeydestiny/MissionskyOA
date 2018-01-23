using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MissionskyOA.Portal.Common;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class ProjectController : Controller
    {
        private IProjectService ProjectService;
        private IUserService UserService;

        public ProjectController(IProjectService ProjectService, IUserService UserService)
        {
            this.ProjectService = ProjectService;
            this.UserService = UserService;
        }
        //
        // GET: /Project/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="dRequest"></param>
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
            var areaResult = ProjectService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
            };

            return Json(result);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            var userList = new List<SelectListItem>();
            var allUsers = UserService.GetAllUsers();
            //userList.Add(new SelectListItem() { Value = "0", Text = "--Please select management--" });
            foreach (UserModel p in allUsers)
            {
                userList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.EnglishName });
            }
            ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", "0");
            if (id.HasValue && id.Value > 0)
            {
                ViewBag.Title = "编辑项目组信息";
                ViewBag.isAddNewProject = false;
                var model = this.ProjectService.SearchProject(id.Value);
                if (model.ProjectManager != null&&model.ProjectManager==0)
                {
                    ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", model.ProjectManager.ToString());
                }
                return View(model);
            }
            else
            {
                ViewBag.Title = "添加项目组";
                ViewBag.isAddNewProject = true;
                ProjectModel model = new ProjectModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ProjectModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入项目组名称");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        ProjectModel projectModel = new ProjectModel()
                        {
                            Name = model.Name,
                            ProjectManager = model.ProjectManager,
                            ProjectBegin = model.ProjectBegin,
                            ProjectEnd = model.ProjectEnd,
                        };
                        this.ProjectService.UpdateProject(model.Id, projectModel);

                    }
                    else
                    {
                        model.Status = 1;
                        model.CreateUserName = CommonInstance.GetInstance().LoginUser.EnglishName;
                        model.CreatedTime = DateTime.Now;
                        this.ProjectService.AddProject(model);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            var userList = new List<SelectListItem>();
            var allUsers = UserService.GetAllUsers();
            //userList.Add(new SelectListItem() { Value = "0", Text = "--Please select management--" });
            foreach (UserModel p in allUsers)
            {
                userList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.EnglishName });
            }
            ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", (int)model.ProjectManager);
            return View(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                this.ProjectService.DeleteProject(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        /// <summary>
        /// 查找关联此项目的用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RelatedUser([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {
                ViewBag.ConfirmMessage="删除";
                var relatedUser = ProjectService.RelatedProjectUser(Id);
                DataSourceResult result = new DataSourceResult()
                {
                    Data = relatedUser.Data,
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        /// <summary>
        /// 解除用户与项目组的绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnRelatedUserProject(int id)
        {
            try
            {
                ViewBag.ConfirmMessage="解除此用户和项目组绑定";
                this.ProjectService.UnRelatedUserProject(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        
	}
}