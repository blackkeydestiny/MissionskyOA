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
    /// 资产
    /// </summary>
    public class AssetModel : BaseModel
    {
        /// <summary>
        /// 类别Id
        /// </summary>
        [Description("类别Id")]
        public int TypeId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [Description("类别名称")]
        public string TypeName { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Description("用户Id")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public AssetStatus Status { get; set; }

        /// <summary>
        /// 状态显示值
        /// </summary>
        [Description("状态显示值")]
        public string StatusName { get; set; }

        /// <summary>
        /// 资转移记录Id(如果资产处于转移中)
        /// </summary>
        [Description("资转移记录Id(如果资产处于转移中)")]
        public int? TransactionId { get; set; }

        /// <summary>
        /// 资产下详细信息
        /// </summary>
        public List<AssetInfoModel> AssetInfoes { get; set; }
    }

}
