using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Api.ApiException
{
    /// <summary>
    /// 非法请求异常
    /// </summary>
    public class ApiBadRequestException : Exception
    {
        public ApiBadRequestException(string message)
            : base(message)
        {
        }
    }
}