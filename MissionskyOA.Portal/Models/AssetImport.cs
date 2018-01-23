using MissionskyOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace MissionskyOA.Portal.Models
{
    /// <summary>
    /// 资产导入Model
    /// </summary>
    public class AssetImport
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int? TypeId { get; set; }  

        /// <summary>
        /// excel
        /// </summary>
        public HttpPostedFileBase DataFile { get; set; }
    }
}