using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 借阅图书
    /// </summary>
    public class BorrowBookModel
    {
        /// <summary>
        /// 图书ID
        /// </summary>
        [Description("图书ID")]
        public virtual int BookId { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        [Description("二维码")]
        public virtual string BarCode { get; set; }
        
        /// <summary>
        /// 预计归还日期
        /// </summary>
        [Description("预计归还日期")]
        public DateTime ReturnDate { get; set; }
        
        /// <summary>
        /// 借阅者(转移图书给其它用户)
        /// </summary>
        [Description("借阅者")]
        public int? Reader { get; set; }
    }
}
