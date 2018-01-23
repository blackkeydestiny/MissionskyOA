using System.Web.Mvc;
namespace MissionskyOA.Portal.Common
{
    public static class HtmlExtensions
    {

        /// <summary>
        /// 自定义一个@html.Submit()
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value">value属性</param>
        /// <returns></returns>
        public static MvcHtmlString Submit(this HtmlHelper helper, string value, string clas = "btn btn-primary")
        {
            var builder = new TagBuilder("input");
            builder.MergeAttribute("type", "submit");
            builder.MergeAttribute("value", value);
            builder.MergeAttribute("class", clas);
            builder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}