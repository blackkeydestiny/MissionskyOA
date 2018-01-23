using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    public class ListResult<T>
    {
        public List<T> Data { get; set; }

        public int Total { get; set; }
    }
}
