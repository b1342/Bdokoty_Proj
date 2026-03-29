using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Features.Uploads.DTOs;

namespace WorkshowcaseApi.Features.Uploads;

[ApiController]
[Route("api/uploads")]
public sealed class UploadsController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public UploadsController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
    }

    [Authorize]
    [HttpPost("work-media")]
    [ProducesResponseType(typeof(UploadWorkMediaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UploadWorkMediaResponse>> UploadWorkMedia(
        [FromForm] UploadWorkMediaRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            return BadRequest();
        }

        var response = await _fileStorageService.SaveWorkMediaAsync(request.File, cancellationToken);
        return Ok(response);
    }
}
