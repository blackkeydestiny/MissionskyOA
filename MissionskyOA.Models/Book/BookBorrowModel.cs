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
    /// 图书借阅
    /// </summary>
    public class BookBorrowModel : BaseModel
    {
        /// <summary>
        /// 借阅用户
        /// </summary>
        [Description("借阅用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 借阅用户姓名
        /// </summary>
        [Description("借阅用户姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 图书ID
        /// </summary>
        [Description("图书ID")]
        public int BookId { get; set; }

        /// <summary>
        /// 图书名
        /// </summary>
        [Description("图书名")]
        public string BookName { get; set; }
        
        /// <summary>
        /// 状态: 已归还，借阅，续借，遗失
        /// </summary>
        [Description("状态: 已归还，借阅，续借，遗失")]
        public UserBorrowStatus Status { get; set; }
        
        /// <summary>
        /// 借阅日期
        /// </summary>
        [Description("借阅日期")]
        public DateTime BorrowDate { get; set; }

        /// <summary>
        /// 预计归还日期
        /// </summary>
        [Description("预计归还日期")]
        public DateTime ReturnDate { get; set; }
    }
}
