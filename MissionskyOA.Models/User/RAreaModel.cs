using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Models.Members
{
    /// <summary>
    /// 地址模型
    /// </summary>
    public class RAreaModel : BaseModel
    {
        [Description("区域")]
        public string District { get; set; }

        [Description("地址")]
        public string Address { get; set; }
    }
}
