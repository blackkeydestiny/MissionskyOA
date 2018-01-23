using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    public class ApiListResponse<T> : ApiResponse
    {
        public virtual List<T> Result { get; set; }
    }
}
