using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Common.Settings;
using WorkshowcaseApi.Features.Uploads.DTOs.Responses;

namespace WorkshowcaseApi.Features.Uploads.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly FileStorageSettings _settings;

    public LocalFileStorageService(
        IWebHostEnvironment webHostEnvironment,
        IOptions<FileStorageSettings> settings)
    {
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<UploadResponse> SaveWorkMediaAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            throw new ValidationException("File is required.");
        }

        if (file.Length == 0)
        {
            throw new ValidationException("File must not be empty.");
        }

        if (file.Length > _settings.MaxFileSizeBytes)
        {
            throw new ValidationException("File size exceeds the maximum allowed limit.");
        }

        var extension = GetNormalizedExtension(file.FileName);
        var mediaType = GetMediaTypeOrThrow(extension);
        var workMediaRootPath = GetWorkMediaRootPath();

        Directory.CreateDirectory(workMediaRootPath);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var physicalFilePath = Path.Combine(workMediaRootPath, storedFileName);

        await using var stream = new FileStream(
            physicalFilePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await file.CopyToAsync(stream, cancellationToken);

        return new UploadResponse
        {
            MediaUrl = BuildRelativeUrl(storedFileName),
            MediaType = mediaType,
            FileSize = file.Length,
            StoredFileName = storedFileName
        };
    }

    public Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
        {
            return Task.CompletedTask;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var workMediaRootPath = GetWorkMediaRootPath();
        var physicalPath = MapRelativeUrlToPhysicalPath(relativeUrl, workMediaRootPath);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        return Task.CompletedTask;
    }

    private string GetWorkMediaRootPath()
    {
        var configuredRootFolder = _settings.WorkMediaRootFolder?.Trim();
        if (string.IsNullOrWhiteSpace(configuredRootFolder))
        {
            throw new InvalidOperationException("FileStorage:WorkMediaRootFolder is not configured.");
        }

        var webRootPath = _webHostEnvironment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
        }

        var normalizedConfiguredRootFolder = NormalizePath(configuredRootFolder);
        var webRootFolderName = Path.GetFileName(webRootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        if (normalizedConfiguredRootFolder.Equals(webRootFolderName, StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFullPath(webRootPath);
        }

        if (normalizedConfiguredRootFolder.StartsWith($"{webRootFolderName}{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
        {
            normalizedConfiguredRootFolder = normalizedConfiguredRootFolder[(webRootFolderName.Length + 1)..];
        }

        return Path.GetFullPath(Path.Combine(webRootPath, normalizedConfiguredRootFolder));
    }

    private string BuildRelativeUrl(string storedFileName)
    {
        var requestPath = NormalizeRequestPath(_settings.WorkMediaRequestPath);
        return $"{requestPath}/{storedFileName}";
    }

    private string MapRelativeUrlToPhysicalPath(string relativeUrl, string workMediaRootPath)
    {
        var sanitizedUrl = relativeUrl.Trim();
        var queryIndex = sanitizedUrl.IndexOfAny(['?', '#']);
        if (queryIndex >= 0)
        {
            sanitizedUrl = sanitizedUrl[..queryIndex];
        }

        sanitizedUrl = sanitizedUrl.Replace('\\', '/');

        var requestPath = NormalizeRequestPath(_settings.WorkMediaRequestPath);
        string relativePath;

        if (sanitizedUrl.StartsWith(requestPath, StringComparison.OrdinalIgnoreCase))
        {
            relativePath = sanitizedUrl[requestPath.Length..].TrimStart('/');
        }
        else
        {
            relativePath = sanitizedUrl.TrimStart('/');
        }

        var combinedPath = Path.GetFullPath(Path.Combine(workMediaRootPath, NormalizePath(relativePath)));
        var normalizedRootPath = workMediaRootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var rootPrefix = $"{normalizedRootPath}{Path.DirectorySeparatorChar}";

        if (!combinedPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) &&
            !combinedPath.Equals(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Invalid media path.");
        }

        return combinedPath;
    }

    private string GetMediaTypeOrThrow(string extension)
    {
        if (ContainsExtension(_settings.AllowedImageExtensions, extension))
        {
            return "image";
        }

        if (ContainsExtension(_settings.AllowedVideoExtensions, extension))
        {
            return "video";
        }

        throw new ValidationException("File extension is not supported.");
    }

    private static bool ContainsExtension(string[]? extensions, string extension)
    {
        if (extensions is null || extensions.Length == 0)
        {
            return false;
        }

        foreach (var allowedExtension in extensions)
        {
            if (string.Equals(NormalizeExtension(allowedExtension), extension, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetNormalizedExtension(string fileName)
    {
        var extension = NormalizeExtension(Path.GetExtension(fileName));
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ValidationException("File extension is required.");
        }

        return extension;
    }

    private static string NormalizeExtension(string? extension)
    {
        return string.IsNullOrWhiteSpace(extension)
            ? string.Empty
            : extension.Trim().ToLowerInvariant();
    }

    private static string NormalizeRequestPath(string? requestPath)
    {
        if (string.IsNullOrWhiteSpace(requestPath))
        {
            throw new InvalidOperationException("FileStorage:WorkMediaRequestPath is not configured.");
        }

        var normalizedRequestPath = requestPath.Trim().Replace('\\', '/').TrimEnd('/');
        if (!normalizedRequestPath.StartsWith('/'))
        {
            normalizedRequestPath = $"/{normalizedRequestPath}";
        }

        return normalizedRequestPath;
    }

    private static string NormalizePath(string path)
    {
        return path
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);
    }
}
