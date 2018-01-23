using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Api.ApiException
{
    /// <summary>
    /// 非法会话请求异常
    /// </summary>
    public class ApiAuthException : Exception
    {
        public ApiAuthException(string message)
            : base(message)
        {
        }
    }
}