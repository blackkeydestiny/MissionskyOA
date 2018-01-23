using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Pager
{
    /// <summary>
    /// 泛型接口：页面列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> 
    {
        // 页面下标
        int PageIndex { get; }

        // 列表数量
        int PageSize { get; }

        // 总记录数
        int TotalCount { get; }

        // 总页数
        int TotalPages { get; }

        // 是否有上一页
        bool HasPreviousPage { get; }

        //是否有下一页
        bool HasNextPage { get; }

        // 返回的
        List<T> Result { get; }
    }
}
