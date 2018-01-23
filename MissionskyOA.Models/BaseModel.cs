using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 基本模型
    /// </summary>
    public class BaseModel
    {
        [Description("主键ID")]
        public int Id { get; set; }
    }
}
