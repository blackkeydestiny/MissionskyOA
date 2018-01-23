using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// Member Token
    /// </summary>
    public partial interface IUserTokenService
    {
        /// <summary>
        /// 登录获取Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TokenModel GetUserToken(LoginModel model);

        /// <summary>
        /// 根据Token获取当前用户
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserModel GetMemberByToken(string token);

        /// <summary>
        /// Validate User Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>true or false</returns>
        bool IsVaild(string token);

        /// <summary>
        /// 设置Token过期
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>true or false</returns>
        bool SetTokenExpired(string token);

        /// <summary>
        /// 检查账号是否被锁定
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsLockStatus(string token);
    }
}
