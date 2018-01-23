using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class DepartmentModel:BaseModel
    {
        public string No { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreateUserName { get; set; }
        public int? DepartmentHead { get; set; }
        public string DepartmentHeadName { get; set; }
        public int? Status { get; set; }
    }
}
