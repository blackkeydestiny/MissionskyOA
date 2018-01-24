using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Security
{
    public interface ICryptology
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Encrypt(string input);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Decrypt(string input);
    }
}
