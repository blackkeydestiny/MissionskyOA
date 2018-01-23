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
    /// 盘点记录
    /// </summary>
    public class AssetInventoryRecordModel : BaseModel
    {
        [Description("盘底主表Id")]
        public int InventoryId { get; set; }

        [Description("用户Id")]
        public int UserId { get; set; }

        [Description("资产Id")]
        public int? AssetId { get; set; }

        [Required]
        [Description("盘点扫描的条码")]
        public string ScanCode { get; set; }
    }

}
