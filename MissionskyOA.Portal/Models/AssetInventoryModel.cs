using MissionskyOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace MissionskyOA.Portal.Models
{
    /// <summary>
    /// 资产盘点视图模型
    /// </summary>
    public class AssetInventoryViewModel
    {
        /// <summary>
        /// 盘点任务Id
        /// </summary>
        public int InventoryId { get; set; }  

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<AssetInventoryModel> List { get; set; }
    }
}