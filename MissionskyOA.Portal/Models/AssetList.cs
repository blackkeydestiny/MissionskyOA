using MissionskyOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace MissionskyOA.Portal.Models
{
    /// <summary>
    /// 资产列表Model
    /// </summary>
    public class AssetList
    {

        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// 条码属性Id
        /// </summary>
        public int BarCodeAttrId { get; set; }

        /// <summary>
        /// 购买日期
        /// </summary>
        public DateTime? BuyDate { get; set; }

        /// <summary>
        /// 使用人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public IPagedList<AssetModel> List { get; set; }
    }
}