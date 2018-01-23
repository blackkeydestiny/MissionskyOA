using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Data
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
