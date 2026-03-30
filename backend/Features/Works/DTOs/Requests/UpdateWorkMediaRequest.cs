namespace WorkshowcaseApi.Features.Works.DTOs.Requests;

public sealed class UpdateWorkMediaRequest
{
    public string? MediaUrl { get; set; }

    public string? ExternalMediaId { get; set; }

    public int? SortOrder { get; set; }

    public string? Caption { get; set; }

    public bool? IsCover { get; set; }
}
