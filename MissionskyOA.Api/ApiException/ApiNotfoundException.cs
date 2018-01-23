using System;

namespace MissionskyOA.Api.ApiException
{
    /// <summary>
    /// 数据记录不存在异常
    /// </summary>
    public class ApiNotfoundException : Exception
    {
        public ApiNotfoundException(string message)
            : base(message)
        {
        }
    }
}