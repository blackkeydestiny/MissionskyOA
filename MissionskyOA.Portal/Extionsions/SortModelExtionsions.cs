using Kendo.Mvc;
using MissionskyOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MissionskyOA
{
    public static class SortModelExtionsions
    {
        public static SortModel ToSortModel(this SortDescriptor sort)
        {
            SortModel model = new SortModel()
            {
                Member = sort.Member
            };

            switch (sort.SortDirection)
            {
                case System.ComponentModel.ListSortDirection.Ascending:
                    model.Direction = SortDirection.Ascending;
                    break;
                case System.ComponentModel.ListSortDirection.Descending:
                    model.Direction = SortDirection.Descending;
                    break;
                default:
                    break;
            }

            return model;
        }

        public static FilterModel ToSortModel(this FilterDescriptor filter)
        {
            FilterModel model = new FilterModel()
            {
                Member = filter.Member,
                ConvertedValue = filter.ConvertedValue != null ? filter.ConvertedValue.ToString() : "",
                Operator = filter.Operator.ToString(),
                Value = filter.Value != null ? filter.Value.ToString() : "",
            };
            return model;
        }
    }
}