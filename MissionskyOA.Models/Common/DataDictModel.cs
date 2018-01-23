using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public class DataDictModel : BaseModel
    {
        /// <summary>
        /// 类型: 图书分类, 设备分类等
        /// </summary>
        [Description("类型: 图书分类, 设备分类等")]
        public string Type { get; set; }

        /// <summary>
        /// 字典项
        /// </summary>
        [Description("字典项")]
        public string Value { get; set; }

        /// <summary>
        /// 字典文本
        /// </summary>
        [Description("字典文本")]
        public string Text { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [Description("父节点")]
        public string Parent { get; set; }
    }
}
