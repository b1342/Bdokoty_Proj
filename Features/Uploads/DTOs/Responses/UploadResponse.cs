namespace WorkshowcaseApi.Features.Uploads.DTOs.Responses;

public sealed class UploadResponse
{
    public string MediaUrl { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public long FileSize { get; set; }

    public string StoredFileName { get; set; } = null!;
}
