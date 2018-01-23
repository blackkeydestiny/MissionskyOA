using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    public class ApiPagingListResponse<T>:ApiListResponse<T>
    {
        public Page Page { get; set; }
    }
}
