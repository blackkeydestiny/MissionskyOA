using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Pager
{
    /// <summary>
    /// Paged list
    /// 泛型类
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [Serializable]
    public class PagedList<T> : IPagedList<T>
    {
       
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int total = source.Count();
            this.TotalCount = total;

            this.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;            
            this.Result = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source 记录数</param>
        /// <param name="pageIndex">Page index 页码</param>
        /// <param name="pageSize">Page size 当前页的记录数</param>
        public PagedList(IList<T> source, int pageIndex, int pageSize)
        {
            // 
            TotalCount = source.Count();

            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
            {
                TotalPages++;
            }
                
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            //
            this.Result = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Result = source.ToList();
        }


        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
       
        public List<T> Result
        {
            get;
            private set;
        }
    }
}
