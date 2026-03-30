using System;
using WorkshowcaseApi.Common.Enums;

namespace WorkshowcaseApi.Domain.Works;

public sealed class WorkMedia
{
    public Guid Id { get; set; }

    public Guid WorkId { get; set; }

    public Work Work { get; set; } = null!;

    public WorkMediaType MediaType { get; set; }

    public string MediaUrl { get; set; } = string.Empty;

    public string? ExternalMediaId { get; set; }

    public int SortOrder { get; set; }

    public string? Caption { get; set; }

    public bool IsCover { get; set; }

    public DateTime CreatedAt { get; set; }
}
