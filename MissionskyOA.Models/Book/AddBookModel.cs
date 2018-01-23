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
    /// 添加图书
    /// </summary>
    public class AddBookModel
    {
        /// <summary>
        /// 书名
        /// </summary>
        [Description("书名")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Description("编码")]
        public string ISBN { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public string Type { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Description("简介")]
        public string Desc { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Description("作者")]
        public string Author { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Description("来源")]
        public BookSource Source { get; set; }

        /// <summary>
        /// 捐赠用户
        /// </summary>
        [Description("捐赠用户")]
        public int? Donor { get; set; }
    }
}
