using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Features.Uploads.DTOs.Requests;
using WorkshowcaseApi.Features.Uploads.DTOs.Responses;
using WorkshowcaseApi.Features.Uploads.Services;

namespace WorkshowcaseApi.Features.Uploads.Controllers;

[ApiController]
[Route("api/uploads")]
public sealed class UploadsController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadsController(IUploadService uploadService)
    {
        _uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
    }

    [Authorize]
    [HttpPost("work-media")]
    [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UploadResponse>> UploadWorkMedia(
        [FromForm] UploadRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            return BadRequest();
        }

        var response = await _uploadService.UploadWorkMediaAsync(request.File, cancellationToken);
        return Ok(response);
    }
}
