using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;

namespace MissionskyOA.Api.Common
{
    public static class HttpUtil
    {
        /// <summary>
        ///  INT Timeout
        /// </summary>
        public static readonly int Timeout = 5000;

        /// <summary>
        /// Default User Agent
        /// </summary>
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>
        /// Default Encoding
        /// </summary>
        private static Encoding defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// POST method.
        /// </summary>
        /// <param name="url">post URL</param>
        /// <param name="postData">post data</param>
        /// <param name="timeout">Timeout. The default is 5000</param>
        /// <param name="encoding">The encoding format, the default format UTF-8</param>
        /// <returns>string post data</returns>
        public static string Post(string url, string postData, int? timeout = null, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            timeout = timeout ?? HttpUtil.Timeout;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = timeout.Value;

            byte[] data = encoding.GetBytes(postData);
            request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                return "exception";
            }
        }
    }
}