using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 图书归类模型
    /// </summary>
    public class BookISBNGroupModel
    {
        [Description("书籍数量")]
        public int amount { get; set; }
        [Description("ISBN编码")]
        public string ISBN { get; set; }
        [Description("会议")]
        public IList<BookModel> books { get; set; }
    }
}
