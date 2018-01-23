using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 资产转移Model
    /// </summary>
    public class AssetTransactionModel : BaseModel
    {
        /// <summary>
        /// 资产Id
        /// </summary>
        [Description("资产Id")]
        [Required]
        public int AssetId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [Description("业务类型")]
        public BusinessType BusinessType { get; set; }

        /// <summary>
        /// 转出用户Id
        /// </summary>
        [Description("转出用户Id")]
        [Required]
        public int OutUserId { get; set; }

        /// <summary>
        ///转出用户名 
        /// </summary>
        [Description("转出用户名")]
        public string OutUserName { get; set; }

        /// <summary>
        /// 转出用户是否确认
        /// </summary>
        [Description("转出用户是否确认")]
        public bool? OutUserIsConfirm { get; set; }

        /// <summary>
        /// 转入用户Id
        /// </summary>
        [Description("转入用户Id")]
        [Required]
        public int InUserId { get; set; }

        /// <summary>
        ///转入用户名 
        /// </summary>
        [Description("转入用户名")]
        public string InUserName { get; set; }

        /// <summary>
        /// 转入用户是否确认
        /// </summary>
        [Description("转入用户是否确认")]
        public bool? InUserIsConfirm { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public AssetTransactionStatus Status { get; set; }

        /// <summary>
        /// 资产对象
        /// </summary>
        [Description("资产对象")]
        public AssetModel Asset { get; set; }
    }
}
