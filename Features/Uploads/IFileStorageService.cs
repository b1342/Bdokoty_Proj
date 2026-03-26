using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WorkshowcaseApi.Features.Uploads.DTOs;

namespace WorkshowcaseApi.Features.Uploads;

public interface IFileStorageService
{
    Task<UploadWorkMediaResponse> SaveWorkMediaAsync(IFormFile file, CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
