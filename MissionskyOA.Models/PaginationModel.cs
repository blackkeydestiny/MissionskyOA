using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    public class PaginationModel<T>
    {
        public IEnumerable<T> Result { get; set; }

        public Page Page { get; set; }
    }

    public class Page
    {

        public int? PageIndex { get; set; }

        public int? PageSize { get; set; }

        public int? TotalCount { get; set; }

        public int? TotalPages { get; set; }

        public bool? HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        public bool? HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
