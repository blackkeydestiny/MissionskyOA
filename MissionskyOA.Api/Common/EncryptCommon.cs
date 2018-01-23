using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web.Security;

namespace MissionskyOA.Api.Common
{
    public class EncryptCommon
    {
        private SymmetricAlgorithm mCSP;
        private const string CIV = "Mi9l/+7Zujhy12se6Yjy111A";
        private const string CKEY = "jkHuIy9D/9i=";
        private const char splitStr = '/';
        private const string salt = "jkHuIy9Dsdjkfd";

        public EncryptCommon()
        {
            mCSP = new DESCryptoServiceProvider();
        }

        public string EncryptString(string Value, string type)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            ct = mCSP.CreateEncryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));
            byt = Encoding.UTF8.GetBytes(this.AddSplitStr(Value, type));
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }


        public string[] DecryptString(string Value)
        {
            if (Value == string.Empty) return new string[] { string.Empty };

            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            ct = mCSP.CreateDecryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));
            byt = Convert.FromBase64String(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return this.SplitStr(Encoding.UTF8.GetString(ms.ToArray()));
        }

        private string AddSplitStr(string value, string type)
        {
            StringBuilder str = new StringBuilder();
            str.Append(type);
            str.Append(splitStr);
            str.Append(value);
            str.Append(splitStr);
            str.Append(Guid.NewGuid().ToString());
            return str.ToString();
        }

        private string[] SplitStr(string value)
        {
            return value.Split(splitStr);
        }


        public string GenerateMD5(string str)
        {
            //string md5Str = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            //return md5Str.Insert(md5Str.Length / 2, salt);

            var csp = new MD5CryptoServiceProvider();
            byte[] value = Encoding.UTF8.GetBytes(str);
            byte[] hash = csp.ComputeHash(value);
            csp.Clear();

            var sb = new StringBuilder();
            for (int i = 0, len = hash.Length; i < len; i++)
            {
                sb.Append(hash[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString().ToLower().Insert(sb.ToString().Length / 2, salt);
        }

        #region Base64
        public string Base64Encode(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public string Base64Decode(string str)
        {
            byte[] outputb = Convert.FromBase64String(str);
            return Encoding.Default.GetString(outputb);
        }

        #endregion
    }
}
