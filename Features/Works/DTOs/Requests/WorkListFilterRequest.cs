using System;

namespace WorkshowcaseApi.Features.Works.DTOs.Requests;

public sealed class WorkListFilterRequest
{
    public Guid? PrimaryCategoryId { get; set; }

    public string? SpaceType { get; set; }

    public string? CreatedByType { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
