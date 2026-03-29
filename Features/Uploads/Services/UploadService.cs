using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WorkshowcaseApi.Features.Uploads.DTOs.Responses;

namespace WorkshowcaseApi.Features.Uploads.Services;

public sealed class UploadService : IUploadService
{
    private readonly IFileStorageService _fileStorageService;

    public UploadService(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
    }

    public Task<UploadResponse> UploadWorkMediaAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        return _fileStorageService.SaveWorkMediaAsync(file, cancellationToken);
    }
}
