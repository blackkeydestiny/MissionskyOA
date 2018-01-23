using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Api.ApiException
{
    /// <summary>
    /// 数据验证失败异常
    /// </summary>
    public class ApiDataValidationException : Exception
    {
        public ApiDataValidationException(string message)
            : base(message)
        {
        }
    }
}