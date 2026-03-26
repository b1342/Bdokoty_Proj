namespace WorkshowcaseApi.Features.Works.DTOs;

public sealed class AddWorkMediaRequest
{
    public string MediaType { get; set; } = null!;

    public string MediaUrl { get; set; } = null!;

    public string? ExternalMediaId { get; set; }

    public int SortOrder { get; set; } = 0;

    public string? Caption { get; set; }

    public bool IsCover { get; set; }
}
