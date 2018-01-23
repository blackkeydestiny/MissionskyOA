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
    /// 资产详细信息
    /// </summary>
    public class AssetInfoModel : BaseModel
    {
        /// <summary>
        /// 资产Id
        /// </summary>
        [Description("资产Id")]
        public int AssetId { get; set; }


        /// <summary>
        /// 属性Id
        /// </summary>
        [Description("属性Id")]
        public int AttributeId { get; set; }

        /// <summary>
        /// 属性内容
        /// </summary>
        [Description("属性内容")]
        public string AttributeValue { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Description("属性名称")]
        public string AttributeName { get; set; }
    }

}
