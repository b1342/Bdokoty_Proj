using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WorkshowcaseApi.Features.Uploads.DTOs.Responses;

namespace WorkshowcaseApi.Features.Uploads.Services;

public interface IUploadService
{
    Task<UploadResponse> UploadWorkMediaAsync(IFormFile file, CancellationToken cancellationToken = default);
}
