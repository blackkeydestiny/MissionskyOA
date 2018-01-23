using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MissionskyOA.Api.Filter;

namespace MissionskyOA.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountController : BaseController
    {
        private IUserTokenService UserTokenService { get; set; }
        private IUserService UserService { get; set; }
        private ICryptology Cryptology { get; set; }

        public AccountController(IUserTokenService userTokenService, IUserService userService, ICryptology cryptology)
        {
            this.UserTokenService = userTokenService;
            this.UserService = userService;
            this.Cryptology = cryptology;
        }

        /// <summary>
        /// 账户登录 (邮箱+密码 或 QQ)
        /// </summary>
        /// <param name="model">用户基本信息</param>
        /// <returns>返回会话令牌</returns>
        [Route("login")]
        [HttpPost]
        public ApiResponse<TokenModel> Login(LoginModel model)
        {
            // 1. 检查输入参数，邮箱和密码 或 FacebookID 是必须参数
            if (model == null || ((String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Password)) && (String.IsNullOrEmpty(model.Phone) || String.IsNullOrEmpty(model.Password)) && String.IsNullOrEmpty(model.QQID)))
            {
                throw new ApiBadRequestException("Invalid account.");
            }

            // 2. 检查邮箱的格式
            if (!String.IsNullOrEmpty(model.Email) && !model.Email.IsEmail())
            {
                throw new ApiDataValidationException("Invalid email");
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                model.Password = Cryptology.Encrypt(model.Password);
            }

            return new ApiResponse<TokenModel>
            {
                Result = this.UserTokenService.GetUserToken(model)
            };
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns>true or fase</returns>
        [Route("logout")]
        [HttpGet]
        public ApiResponse<bool> Logout()
        {
            return new ApiResponse<bool>
            {
                Result = this.UserTokenService.SetTokenExpired(this.Token)
            };
        }
        
        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="model">用户新旧密码</param>
        /// <returns>是否更新成功</returns>
        [Route("updatepassword")]
        [HttpPut]
        [RequestBodyFilter]
        public ApiResponse<bool> UpdatePassowrd(UpdatePasswordModel model)
        {
            if (model == null)
            {
                throw new Exception("The request body cant't be null.");
            }

            //旧密码
            if (string.IsNullOrEmpty(model.Password))
            {
                throw new Exception("Invalid old password.");
            }

            //新密码
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                throw new Exception("Invalid new password.");
            }

            //加密密码
            model.Password = Cryptology.Encrypt(model.Password);
            model.NewPassword = Cryptology.Encrypt(model.NewPassword);

            //更新密码
            return new ApiResponse<bool>
            {
                Result = this.UserService.UpdatePassowrd(this.Token, model) 
            };
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="model">用户邮箱或电话</param>
        /// <returns>是否重置成功</returns>
        [Route("resetpassword")]
        [HttpPost]
        public ApiResponse<bool> ResetPassword(ResetPasswordModel model)
        {
            //重置密码
            return new ApiResponse<bool>
            {
                Result = this.UserService.ResetPassword(model)
            };
        }
    }
}
