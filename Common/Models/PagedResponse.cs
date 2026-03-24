using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Common.Models;

public sealed class PagedResponse<T>
{
    public PagedResponse(IReadOnlyCollection<T> items, int totalCount, int page, int pageSize)
    {
        Items = items ?? Array.Empty<T>();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public IReadOnlyCollection<T> Items { get; }

    public int TotalCount { get; }

    public int Page { get; }

    public int PageSize { get; }
}
