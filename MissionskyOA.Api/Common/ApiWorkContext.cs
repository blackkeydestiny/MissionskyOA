using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MissionskyOA.Models;

namespace MissionskyOA.Api
{
    /// <summary>
    /// API Context 对象
    /// </summary>
    public class ApiWorkContext
    {
        /// </summary>
        /// 会话ID
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// 私有静态实例对象
        /// </summary>
        private static ApiWorkContext _instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        private ApiWorkContext(){}

        /// <summary>
        /// 静态实例化单实例
        /// </summary>
        /// <returns></returns>
        public static ApiWorkContext Instance()
        {
            if (_instance == null)
            {
                _instance = new ApiWorkContext();
            }

            return _instance;
        }
    }
}