using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;

namespace MissionskyOA.Resources
{
    public class Resource
    {
        #region Fields

        private const string USER_PREFERRED_CULTURE_SESSION_KEY = "UserPreferredCulture";
        private const string USER_PREFERRED_CULTURE_COOKIE_KEY = "UserPreferredCulture";
        private static CultureInfo defaultSysCulture = new CultureInfo("en-US");
        private static global::System.Resources.ResourceManager resourceMan;
        private static List<LangValue> _cache = new List<LangValue>();

        private struct LangValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string Lang { get; set; }
        }

        #endregion Fields

        #region Construction

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    resourceMan = new global::System.Resources.ResourceManager("MissionskyOA.Resources.Resource", typeof(Resource).Assembly);
                }

                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static CultureInfo UserPreferredCulture
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    // 获取默认的culture
                    return GetDefaultCulture();
                }

                CultureInfo culture = null;

                if (HttpContext.Current.Session != null)
                {
                    // 从session中获取CultureInfo
                    culture = HttpContext.Current.Session[USER_PREFERRED_CULTURE_SESSION_KEY] as CultureInfo;
                }

                if (culture == null)
                {
                    //
                    culture = GetDefaultCulture();
                }

                return culture;
            }

            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    // 向session里设置值
                    HttpContext.Current.Session[USER_PREFERRED_CULTURE_SESSION_KEY] = value;
                }

                SetCultureToCookie(value);
            }
        }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Gets the default CultureInfo, which is set in configuration(web.config).
        /// If not configured, load the en-US as default CultureInfo.
        /// 获取默认的culture
        /// </summary>
        /// <returns>the default CultureInfo</returns>
        private static CultureInfo GetDefaultCulture()
        {
            try
            {
                string defaultCultString = ConfigurationManager.AppSettings["DefaultCulture"];

                if (!String.IsNullOrWhiteSpace(defaultCultString))
                {
                    return new CultureInfo(defaultCultString);
                }

            }
            catch
            {
            }

            return defaultSysCulture;
        }

        /// <summary>
        /// set culture to cookie
        /// 设置culture到cookie
        /// </summary>
        /// <param name="culture">the culture info</param>
        private static void SetCultureToCookie(CultureInfo culture)
        {
            // 1、判断
            if (HttpContext.Current == null || culture == null)
            {
                return;
            }
            
            // 2、创建cookie对象
            HttpCookie theCookie = new HttpCookie(USER_PREFERRED_CULTURE_COOKIE_KEY);
            //theCookie.HttpOnly = true;

            // 3、为cookie对象设置相应的属性值
            if (HttpContext.Current.Request.Url.Scheme.ToLower() == "https")
            {
                //设置是否为https
                theCookie.Secure = true;
            }
            
            // 设置cookies的值
            theCookie.Value = culture.Name;
            // 设置超期时间
            theCookie.Expires = DateTime.UtcNow.AddDays(90);

            //4、添加到Cookies
            HttpContext.Current.Response.Cookies.Add(theCookie);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Read the resource file and return the string of specified culture
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <param name="culture">The System.Globalization.CultureInfo object that represents the culture for
        /// which the resource is localized. Note that if the resource is not localized
        /// for this culture, the lookup will fall back using the current thread's System.Globalization.CultureInfo.Parent
        /// property, stopping after looking in the neutral culture.If this value is
        /// null, the System.Globalization.CultureInfo is obtained using the current
        /// thread's System.Globalization.CultureInfo.CurrentUICulture property.</param>
        /// <returns>
        /// The value of the resource localized for the specified culture. If a best
        /// match is not possible, null is returned.
        /// </returns>
        public static string GetString(string name, CultureInfo culture)
        {
            return ResourceManager.GetString(name, culture);
        }

        /// <summary>
        /// Returns the value of the specified System.String resource.
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <returns>The value of the resource localized for the caller's current culture settings.
        /// If a match is not possible, null is returned.
        /// </returns>
        public static string GetString(string name)
        {
            return ResourceManager.GetString(name, UserPreferredCulture);
        }

        /// <summary>
        /// Get resource by key 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns>
        /// </returns>
        public static string GetString(string name, string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                var langLower = lang.ToLower();
                if (langLower == "zh-hk")
                {
                    return ResourceManager.GetString(name, new CultureInfo("zh-HK"));
                }
            }

            return ResourceManager.GetString(name, UserPreferredCulture);
        }
        #endregion
    }
}
