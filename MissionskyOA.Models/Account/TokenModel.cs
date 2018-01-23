using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    public class TokenModel
    {
        [Description("Token")]
        public string Token { get; set; }

        [Description("用户Id")]
        public int Id { get; set; }

        [Description("极光推送别名")]
        public string JPushAlias { get; set; }

        [Description("极光推送用户标签")]
        public string JPushTag { get; set; }
    }
}
