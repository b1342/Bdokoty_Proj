using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Works.DTOs;

namespace WorkshowcaseApi.Features.Works;

[ApiController]
[Route("api/works")]
public sealed class WorksController : ControllerBase
{
    private readonly IWorksService _worksService;

    public WorksController(IWorksService worksService)
    {
        _worksService = worksService ?? throw new ArgumentNullException(nameof(worksService));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(WorkResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkResponse>> Create([FromBody] CreateWorkRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _worksService.CreateAsync(callerId, request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkResponse>> GetById([FromRoute] Guid id)
    {
        TryGetCallerId(out var callerId);
        var response = await _worksService.GetByIdAsync(callerId, id);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<WorkListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<WorkListItemResponse>>> GetFeed([FromQuery] WorkListQuery query)
    {
        var response = await _worksService.GetPublishedFeedAsync(query);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkResponse>> Update([FromRoute] Guid id, [FromBody] UpdateWorkRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _worksService.UpdateAsync(callerId, id, request);
        return Ok(response);
    }

    private bool TryGetCallerId(out Guid callerId)
    {
        var callerIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        return Guid.TryParse(callerIdRaw, out callerId);
    }
}
