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
    /// 资产类别
    /// </summary>
    public class AssetTypeModel : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int? Sort { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        [Description("属性")]
        public List<AssetAttributeModel> Attributes { get; set; }
    }
}
