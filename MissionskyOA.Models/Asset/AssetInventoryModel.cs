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
    /// 资产盘点
    /// </summary>
    public class AssetInventoryModel : BaseModel
    {
        /// <summary>
        /// 资产盘点通告标题
        /// </summary>
        [Description("资产盘点通告标题")]
        public string Title { get; set; }


        /// <summary>
        ///  资产盘点通告内容
        /// </summary>
        [Description("资产盘点通告内容")]
        public string Description { get; set; }


        /// <summary>
        ///  盘点任务状态
        /// </summary>
        [Description("盘点任务状态")]
        public AssetInventoryStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("资产Id")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 盘点记录
        /// </summary>
        [Description("盘点记录")]
        public List<AssetInventoryRecordModel> AssetInventoryRecords { get; set; }
    }

}
