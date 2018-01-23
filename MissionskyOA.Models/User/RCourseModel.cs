using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Models
{
    /// <summary>
    /// 课程模型
    /// </summary>
    public class RCourseModel : BaseModel
    {
        [Description("第一级分类")]
        public string Type { get; set; }
        
        [Description("第二级分类")]
        public string SubType { get; set; }

        [Description("课程名")]
        public string CourseName { get; set; }

        [Description("描述")]
        public string Description { get; set; }
    }
}
