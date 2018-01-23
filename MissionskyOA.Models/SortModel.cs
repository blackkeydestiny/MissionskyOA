using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    public class SortModel
    {
        public string Member { get; set; }

        public SortDirection Direction { get; set; }
    }

    public enum SortDirection
    {
        // Summary:
        //     Sorts in ascending order.
        Ascending = 0,
        //
        // Summary:
        //     Sorts in descending order.
        Descending = 1,
    }

    public class FilterModel
    {
        public string ConvertedValue { get; set; }

        public string Member { get; set; }

        public string Operator { get; set; }

        public string Value { get; set; }
    }
}
