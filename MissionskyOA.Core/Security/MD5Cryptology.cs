using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MissionskyOA.Core.Security
{
    public class MD5Cryptology:ICryptology
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Encrypt(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(buffer);
            return BitConverter.ToString(result);            
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Decrypt(string input)
        {
            return null;
        }
    }
}
