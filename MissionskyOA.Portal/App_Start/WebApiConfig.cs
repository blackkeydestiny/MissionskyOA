using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace MissionskyOA.Portal
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.JsonFormatter);
        }

        /// <summary>
        /// Json数据格式化
        /// </summary> 
        public static void JsonFormatters()
        {
            // 创建一个时间格式转换器对象
            var datetimeConverter = new IsoDateTimeConverter();
            // 定义时间格式
            datetimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(datetimeConverter);

            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings = serializerSettings;
            GlobalConfiguration.Configuration.Formatters.Add(jsonFormatter);               
        }
    }
}
