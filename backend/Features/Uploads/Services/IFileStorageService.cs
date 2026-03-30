using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WorkshowcaseApi.Features.Uploads.DTOs.Responses;

namespace WorkshowcaseApi.Features.Uploads.Services;

public interface IFileStorageService
{
    Task<UploadResponse> SaveWorkMediaAsync(IFormFile file, CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
