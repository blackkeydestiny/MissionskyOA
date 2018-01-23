using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 资产属性
    /// </summary>
    public class AssetAttributeModel : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [Description("数据类型")]
        public AssetDataType DataType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int? Sort { get; set; }
        
    }
}
