using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class AcceptProxyModel : BaseModel
    {
        [Description("接收日期")]
        public DateTime Date { get; set; }
        [Description("描述")]
        public string Description { get; set; }
        [Description("附件名称")]
        public string FileName { get; set; }
        [Description("附件内容")]
        public byte[] Content { get; set; }
        [Description("接收人员")]
        public int AcceptUserId { get; set; }

        [Description("接收人员姓名")]
        public string AcceptUserName { get; set; }
        [Description("签收状态")]
        public ExpressStatus Status { get; set; }
        [Description("快递员姓名")]
        public string Courier { get; set; }
        [Description("快递员留言")]
        public string LeaveMessage { get; set; }
        [Description("快递员电话")]
        public string CourierPhone { get; set; }
        [Description("创建人id")]
        public int? CreateUserId { get; set; }

        [Description("代签人姓名")]
        public string CreateUserName { get; set; }
        [Description("最后更新人员id")]
        public int? LastModifyUserId { get; set; }
        [Description("最后更新时间")]
        public DateTime? LastModifyTime { get; set; }
    }
}
