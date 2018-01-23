using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Models.Members
{
    public class CourseListModel
    {
        [Description("第一级分类")]
        public string Type { get; set; }

        [Description("第二级分类")]
        public string SubType { get; set; }
    }
}
