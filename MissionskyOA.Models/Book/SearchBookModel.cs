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
    /// 
    /// </summary>
    public class SearchBookModel
    {
        /// <summary>
        /// 状态: 在库，借出, 遗失
        /// </summary>
        [Description("状态: 在库，借出, 遗失")]
        public BookStatus Status { get; set; }
        
        /// <summary>
        /// 书名
        /// </summary>
        [Description("书名")]
        public string Name { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        [Description("二维码")]
        public string BarCode { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Description("编码")]
        public string ISBN { get; set; }

        /// <summary>
        /// 图书来源
        /// </summary>
        [Description("图书来源")]
        public BookSource Source { get; set; }
    }
}
