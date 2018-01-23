using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web.Security;
using System.Web;


namespace MissionskyOA.Api
{
    /// <summary>
    /// Summary description for FileUtil
    /// </summary>
    public static class FileUtil
    {
        #region Properties

        /// <summary>
        /// Gets Application root, ends with /
        /// </summary>
        public static string ApplicationRoot
        {
            get
            {
                return GetApplicationRoot(HttpContext.Current);
            }
        }

        /// <summary>
        /// Gets Application root, ends with /
        /// </summary>
        public static string GetApplicationRoot(HttpContext context)
        {
            string sRoot = context.Request.ApplicationPath;

            if (String.IsNullOrEmpty(sRoot))
            {
                sRoot = ".";
            }

            if (!sRoot.EndsWith("/", StringComparison.InvariantCulture))
            {
                sRoot += "/";
            }

            return sRoot;
        }


        #endregion

        #region Methods

        /// <summary>
        /// return full vitual path with application root appended.
        /// </summary>
        /// <param name="partialPath"></param>
        /// <returns></returns>
        public static string AppendApplicationRoot(string partialPath)
        {
            return CombineWebPath(ApplicationRoot, partialPath);
        }

        /// <summary>
        /// Appends the absolute path, contains host name, port and virtual path.
        /// For example: http://hostname:1234/virtualPath/ + partialPath
        /// </summary>
        /// <param name="partialPath">The partial path.</param>
        /// <returns></returns>
        public static string AppendAbsolutePath(string partialPath)
        {
            string absolutePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
            string applicationPath = AppendApplicationRoot(partialPath);

            return CombineWebPath(absolutePath, applicationPath);
        }

        /// <summary>
        /// combine web path
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string CombineWebPath(string path1, string path2)
        {
            if (path1 != null && path2 != null)
            {
                if (!path1.EndsWith("/", StringComparison.InvariantCulture))
                {
                    path1 += "/";
                }

                if (path2.StartsWith("/", StringComparison.InvariantCulture))
                {
                    path2 = path2.Substring(1);
                }
            }

            return path1 + path2;
        }

        /// <summary>
        /// Delete CSV File
        /// </summary>
        /// <param name="filePath">CSV File Path</param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        #endregion Methods
    }
}