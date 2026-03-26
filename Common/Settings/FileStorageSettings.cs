namespace WorkshowcaseApi.Common.Settings;

public sealed class FileStorageSettings
{
    public string WorkMediaRootFolder { get; set; } = null!;

    public string WorkMediaRequestPath { get; set; } = null!;

    public long MaxFileSizeBytes { get; set; }

    public string[] AllowedImageExtensions { get; set; } = null!;

    public string[] AllowedVideoExtensions { get; set; } = null!;
}
