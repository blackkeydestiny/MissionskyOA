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
    /// 资产资产Model
    /// </summary>
    public class AssetSearchModel
    {
        /// <summary>
        /// 条码
        /// </summary>
        [Description("条码/编码")]
        public string BarCode { get; set; }

        /// <summary>
        /// 类别Id
        /// </summary>
        [Description("类别Id")]
        public int? TypeId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public AssetStatus? Status { get; set; }

        /// <summary>
        /// 采购日期(按月查)
        /// </summary>
        public DateTime? BuyDate { get; set; }

        /// <summary>
        /// 使用人姓名(后台查询)
        /// </summary>
        public string UserName { get; set; }
    }
   
}
