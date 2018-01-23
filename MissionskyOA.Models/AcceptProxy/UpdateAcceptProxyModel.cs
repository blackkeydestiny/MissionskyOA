using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
namespace MissionskyOA.Models
{
    public class UpdateAcceptProxyModel
    {
        [Description("描述")]
        public string Description { get; set; }
        [Description("代签人员")]
        public int AcceptUserId { get; set; }
        [Description("签收状态")]
        public ExpressStatus Status { get; set; }
        
    }
}
