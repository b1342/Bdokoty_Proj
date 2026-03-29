using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Features.Works.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;
using WorkshowcaseApi.Features.Works.Services;

namespace WorkshowcaseApi.Features.Works.Controllers;

[ApiController]
[Route("api/works/{workId:guid}/media")]
public sealed class WorkMediaController : ControllerBase
{
    private readonly IWorkMediaService _workMediaService;

    public WorkMediaController(IWorkMediaService workMediaService)
    {
        _workMediaService = workMediaService ?? throw new ArgumentNullException(nameof(workMediaService));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(WorkMediaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkMediaResponse>> Create([FromRoute] Guid workId, [FromBody] AddWorkMediaRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _workMediaService.AddMediaAsync(callerId, workId, request);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [Authorize]
    [HttpPut("{mediaId:guid}")]
    [ProducesResponseType(typeof(WorkMediaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkMediaResponse>> Update([FromRoute] Guid workId, [FromRoute] Guid mediaId, [FromBody] UpdateWorkMediaRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _workMediaService.UpdateMediaAsync(callerId, workId, mediaId, request);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{mediaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid workId, [FromRoute] Guid mediaId)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        await _workMediaService.DeleteMediaAsync(callerId, workId, mediaId);
        return NoContent();
    }

    private bool TryGetCallerId(out Guid callerId)
    {
        var callerIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        return Guid.TryParse(callerIdRaw, out callerId);
    }
}
