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
    public class RoleController : Controller
    {
         private IRoleService RoleService;
        private IUserRoleService UserRoleService;
        //
        // GET: /Area/

        public RoleController(IRoleService RoleService, IUserRoleService UserRoleService)
        {
            this.RoleService = RoleService;
            this.UserRoleService = UserRoleService;
        }
        //
        // GET: /Role/
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
            var noticeResult = RoleService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = noticeResult.Data,
                Total = noticeResult.Total
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
            if (id.HasValue && id.Value > 0)
            {
                ViewBag.Title = "编辑角色信息";
                ViewBag.isAddNewRole = false;
                var model = this.RoleService.SearchRole(id.Value);
                return View(model);
            }
            else
            {
                ViewBag.Title = "添加角色信息";
                ViewBag.isAddNewRole = true;
                RoleModel model = new RoleModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(RoleModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }
            if(string.IsNullOrEmpty(model.Abbreviation))
            {
                ModelState.AddModelError("Abbreviation", "英文简称不能为空");
            }
            if (string.IsNullOrEmpty(model.RoleName))
            {
                ModelState.AddModelError("RoleName", "角色名不能为空");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        RoleModel updateUserModel = new RoleModel()
                        {
                            RoleName=model.RoleName,
                            Status=model.Status
                        };
                        this.RoleService.UpdateRole(model.Id, updateUserModel);
                          
                    }
                    else
                    {
                        model.CreatedTime = DateTime.Now;
                        model.CreateUser = CommonInstance.GetInstance().LoginUser.EnglishName;
                        model.IsInitRole = 0;
                        this.RoleService.AddRole(model);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

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
                this.RoleService.DeleteRole(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        /// <summary>
        /// 查找关联此角色的用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RelatedUser([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {
                var relatedUser = RoleService.RelatedUser(Id);
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
        /// 解除绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnRelatedUserRole(int id)
        {
            try
            {
                this.UserRoleService.DeleteUserRole(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        
	}
}