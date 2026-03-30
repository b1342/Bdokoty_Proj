using System;

namespace WorkshowcaseApi.Features.Works.DTOs.Responses;

public sealed class WorkMediaResponse
{
    public Guid Id { get; set; }

    public Guid WorkId { get; set; }

    public string MediaType { get; set; } = null!;

    public string MediaUrl { get; set; } = null!;

    public string? ExternalMediaId { get; set; }

    public int SortOrder { get; set; }

    public string? Caption { get; set; }

    public bool IsCover { get; set; }

    public DateTime CreatedAt { get; set; }
}
