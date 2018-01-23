using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using MissionskyOA.Core.Enum;
using System.Text.RegularExpressions;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class UserController : Controller
    {
        IUserService UserService;
        IProjectService ProjectService;
        IAttendanceSummaryService AttendanceSummaryService;
        IRoleService RoleService;
        //
        // GET: /Area/

        public UserController(IUserService userService, IProjectService ProjectService, IAttendanceSummaryService AttendanceSummaryService,IRoleService RoleService)
        {
            this.UserService = userService;
            this.ProjectService = ProjectService;
            this.AttendanceSummaryService = AttendanceSummaryService;
            this.RoleService = RoleService;
        }

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
            var areaResult = UserService.List(dRequest.Page, dRequest.PageSize, sort, filter);
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
            //project
            var allProjects = ProjectService.SearchAllProject();
            var projectList = new List<SelectListItem>();
            projectList.Add(new SelectListItem() { Value = "0", Text = "--Please select project--" });
            foreach (ProjectModel p in allProjects)
            {
                projectList.Add(new SelectListItem()
                {
                    Text = p.Name,
                    Value = (p.ProjectId).ToString()
                });
            }
            //Super user
            var DirectlySupervisorList = new List<SelectListItem>();
            var allDirectlySuperviso = UserService.GetManagementUsers();
            DirectlySupervisorList.Add(new SelectListItem() { Value = "0", Text = "--Please select supervisor--" });
            foreach (UserModel p in allDirectlySuperviso)
            {
                DirectlySupervisorList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.EnglishName });
            }

            //role
            var RoleList = new List<SelectListItem>();
            var allRoleList = RoleService.SearchAllRole();
            RoleList.Add(new SelectListItem() { Value = "0", Text = "--Please select role--" });
            foreach (RoleModel p in allRoleList)
            {
                RoleList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.RoleName });
            }

            //dept
            var DeptList = new List<SelectListItem>();
            var allDeptList = UserService.GetDets();
            foreach (DepartmentModel p in allDeptList)
            {
                DeptList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.Name });
            }


            ViewData["projectList"] = new SelectList(projectList, "Value", "Text", "0");
            ViewData["deptList"] = new SelectList(DeptList, "Value", "Text", "0");
            ViewData["directlySupervisorList"] = new SelectList(DirectlySupervisorList, "Value", "Text", "0");
            ViewData["roleList"] = new SelectList(RoleList, "Value", "Text", "0");
            
            if (id.HasValue && id.Value > 0)
            {
                ViewBag.Title = "编辑用户信息";
                var model = this.UserService.GetUserDetail(id.Value);
                if (model.ProjectId != null)
                {
                    ViewData["projectList"] = new SelectList(projectList, "Value", "Text", model.ProjectId.ToString());
                
                }
                if (model.DirectlySupervisorId != null)
                {
                    ViewData["directlySupervisorList"] = new SelectList(DirectlySupervisorList, "Value", "Text", model.DirectlySupervisorId.ToString());
                }
                if(model.DeptId!=null)
                {
                    ViewData["deptList"] = new SelectList(DeptList, "Value", "Text", model.DeptId.ToString());
                }
                if(model.Role!=0)
                {
                    ViewData["roleList"] = new SelectList(RoleList, "Value", "Text", model.Role.ToString());
                } 
                return View(model);
            }  
            else
            {
                ViewBag.Title = "添加用户信息";
                UserModel model = new UserModel();
                //初始值
                model.JoinDate = DateTime.Now;
                model.TodayStatus = UserTodayStatus.Normal;
                model.Available = true;
                model.Gender = Gender.Male;
                return View(model);
            }
        }

        public ActionResult EditingPopup_Update([DataSourceRequest] DataSourceRequest request,  AttendanceSummaryModel attendance)
        {
            if (attendance != null && ModelState.IsValid)
            {
                AttendanceSummaryService.ModifyAttendanceSummary(attendance);
            }

            return Json(new[] { attendance }.ToDataSourceResult(request, ModelState));
        }

         /// <summary>
        /// 取得用户年假信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAttendanceSummary([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {
                var attendanceSummary = AttendanceSummaryService.GetAttendanceSummariesByUserId(Id);
                DataSourceResult result = new DataSourceResult()
                {
                    Data = attendanceSummary
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.ChineseName))
            {
                ModelState.AddModelError("ChineseName", "请输入中文名");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.ChineseName, @"[\u4E00-\u9FFF]+"))
                {
                    ModelState.AddModelError("ChineseName", "请输入中文");
                }

            }
            if (string.IsNullOrEmpty(model.No))
            {
                ModelState.AddModelError("No", "请输入工号");
            }
            if (string.IsNullOrEmpty(model.Position))
            {
                ModelState.AddModelError("Position", "请输入岗位");
            }

            if (model.Role==0)
            {
                ModelState.AddModelError("Role", "请选择角色");
            }
            if (string.IsNullOrEmpty(model.EnglishName))
            {
                ModelState.AddModelError("EnglishName", "请输入英文名");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.EnglishName, @"^[A-Za-z][A-Za-z\s]*[A-Za-z]$")) 
                {
                    ModelState.AddModelError("EnglishName", "英文名格式不正确");
                }
                
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.Phone, @"1[0-9]{10}"))
                {
                    ModelState.AddModelError("Phone", "手机号码格式不正确");
                }
            }
            if (!string.IsNullOrEmpty(model.QQID))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.QQID, @"[1-9][0-9]{4,}"))
                {
                    ModelState.AddModelError("QQID", "QQ码格式不正确");
                }
            }
            if (!string.IsNullOrEmpty(model.No))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.No, @"^([a-zA-Z]*?)((?:[0-9]+(?:[0-9]|[a-zA-Z])*)*)$"))
                {
                    ModelState.AddModelError("No", "工号只能为英文和数字");
                }
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "请输入邮箱");
            }
            else
            {
                Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if(!regex.IsMatch(model.Email))
                {
                    ModelState.AddModelError("Email", "邮箱格式不正确");
                }       
            }
            if(model.DeptId==0)
            {
                ModelState.AddModelError("DeptId", "请选择部门");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        if (model.ServerYear < 0)
                        {
                            model.ServerYear = Math.Abs(model.ServerYear);
                        }
                        if (model.ServerYear >= 0 && model.ServerYear < 10)
                        {
                            model.ServerYearType = UserServiceYearType.TenYears;
                        }
                        else if (model.ServerYear >= 10 && model.ServerYear < 20)
                        {
                            model.ServerYearType = UserServiceYearType.TwentyYears;
                        }
                        else if (model.ServerYear >= 20)
                        {
                            model.ServerYearType = UserServiceYearType.ThirtyYears;
                        }

                        UpdateUserModel updateUserModel = new UpdateUserModel()
                        {
                            No=model.No,
                            ChineseName = model.ChineseName,
                            EnglishName=model.EnglishName,
                            Email = model.Email,
                            QQID = model.QQID,
                            Gender = model.Gender,
                            DirectlySupervisorId = model.DirectlySupervisorId,
                            Phone = model.Phone,
                            ProjectId = model.ProjectId,
                            Status = model.Status,
                            Available=model.Available,
                            TodayStatus=model.TodayStatus,
                            Position=model.Position,
                            Role =model.Role,
                            JoinDate=model.JoinDate,
                            DeptId=model.DeptId,
                            ServerYear = model.ServerYear,
                            ServerYearType = model.ServerYearType
                        };
                        this.UserService.UpdateUser(model.Id, updateUserModel);
                          
                    }
                    else
                    {
                        this.UserService.AddUser(model);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            //project
            var allProjects = ProjectService.SearchAllProject();
            var projectList = new List<SelectListItem>();
            projectList.Add(new SelectListItem() { Value = "0", Text = "--Please select project--" });
            foreach (ProjectModel p in allProjects)
            {
                projectList.Add(new SelectListItem()
                {
                    Text = p.Name,
                    Value = (p.ProjectId).ToString()
                });
            }
            //Super user
            var DirectlySupervisorList = new List<SelectListItem>();
            var allDirectlySuperviso = UserService.GetManagementUsers();
            DirectlySupervisorList.Add(new SelectListItem() { Value = "0", Text = "--Please select supervisor--" });
            foreach (UserModel p in allDirectlySuperviso)
            {
                DirectlySupervisorList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.EnglishName });
            }

            //role
            var RoleList = new List<SelectListItem>();
            var allRoleList = RoleService.SearchAllRole();
            RoleList.Add(new SelectListItem() { Value = "0", Text = "--Please select role--" });
            foreach (RoleModel p in allRoleList)
            {
                RoleList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.RoleName });
            }

            //dept
            var DeptList = new List<SelectListItem>();
            var allDeptList = UserService.GetDets();
            foreach (DepartmentModel p in allDeptList)
            {
                DeptList.Add(new SelectListItem() { Value = (p.Id).ToString(), Text = p.Name });
            }


            ViewData["projectList"] = new SelectList(projectList, "Value", "Text", (int)model.ProjectId);
            ViewData["deptList"] = new SelectList(DeptList, "Value", "Text", (int)model.DeptId);
            ViewData["directlySupervisorList"] = new SelectList(DirectlySupervisorList, "Value", "Text", (int)model.DirectlySupervisorId);
            ViewData["roleList"] = new SelectList(RoleList, "Value", "Text", (int)model.Role);
            return View(model);
        }
        
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AttendanceSummaryEdit(int? id)
        {
            ViewBag.Title = "编辑年假信息";
            AttendanceSummaryModel model = new AttendanceSummaryModel();
            return View(model);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAttendanceSummary(AttendanceSummaryModel model)
        {
            try
            {
                this.AttendanceSummaryService.ModifyAttendanceSummary(model);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
                this.UserService.Remove(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
    }
}