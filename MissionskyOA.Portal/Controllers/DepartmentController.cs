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
    public class DepartmentController : Controller
    {
        private IDepartmentService DepartmentService;
        private IUserService UserService;

        public DepartmentController(IDepartmentService DepartmentService,IUserService UserService)
        {
            this.DepartmentService = DepartmentService;
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
            var areaResult = DepartmentService.List(dRequest.Page, dRequest.PageSize, sort, filter);
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
                ViewBag.Title = "编辑部门组信息";
                ViewBag.isAddNew = false;
                var model = this.DepartmentService.GetDepartmentByID(id.Value);
                if (model.DepartmentHead != null && model.DepartmentHead == 0)
                {
                    ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", model.DepartmentHead.ToString());
                }
                return View(model);
            }
            else
            {
                ViewBag.Title = "添加部门";
                ViewBag.isAddNew = true;
                DepartmentModel model = new DepartmentModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(DepartmentModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入部门名称");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        DepartmentModel deparmentModel = new DepartmentModel()
                        {
                            No=model.No,
                            Name = model.Name,
                            DepartmentHead=model.DepartmentHead
                        };
                        this.DepartmentService.UpdateDepartment(model.Id, deparmentModel);

                    }
                    else
                    {
                        model.CreateUserName = CommonInstance.GetInstance().LoginUser.EnglishName;
                        model.CreatedDate = DateTime.Now;
                        model.Status=1;
                        this.DepartmentService.AddDepartment(model);
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
            ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", (int)model.DepartmentHead);
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
                this.DepartmentService.deleteDepartment(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

    }
}
