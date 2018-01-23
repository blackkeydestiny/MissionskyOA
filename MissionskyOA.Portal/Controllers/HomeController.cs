using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MissionskyOA.Models;
using MissionskyOA.Portal.Common;
using MissionskyOA.Services;

namespace MissionskyOA.Portal.Controllers
{
    public class HomeController : Controller
    {
        IUserService UserService;
        IRoleService RoleService;

        public HomeController(IUserService UserService, IRoleService RoleService)
        {
            this.UserService = UserService;
            this.RoleService = RoleService;
        }
        //
        // GET: /Home/

        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrWhiteSpace(model.UserName))
            {
                ModelState.AddModelError(string.Empty, "用户名不能为空.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "密码不能为空.");
                return View(model);
            }

            var userModel = UserService.Login(model);
            if (userModel == null)
            {
                ModelState.AddModelError(string.Empty, "无效的用户名或密码.");
                return View(model);
            }
            else if (!userModel.IsAssetManager && !userModel.IsAdminStaff) //判断登录且用户角色为行政专员或资产管理员
            {
                ModelState.AddModelError(string.Empty, "用户无登录权限.");
                return View(model);
            }
            else
            {
                CommonInstance.GetInstance().LoginUser = userModel;
                FormsAuthentication.SetAuthCookie(model.UserName, false);
                //HttpCookie cookie = new HttpCookie(COOKIE_NAME, model.UserName);
                //cookie.Expires = DateTime.Now.AddDays(30);
                //Response.AppendCookie(cookie);
                return RedirectToAction("Index", "User");
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

    }
}
