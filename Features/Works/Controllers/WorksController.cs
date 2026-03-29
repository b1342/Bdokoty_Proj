using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Works.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;
using WorkshowcaseApi.Features.Works.Services;

namespace WorkshowcaseApi.Features.Works.Controllers;

[ApiController]
[Route("api/works")]
public sealed class WorksController : ControllerBase
{
    private readonly IWorkService _workService;

    public WorksController(IWorkService workService)
    {
        _workService = workService ?? throw new ArgumentNullException(nameof(workService));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(WorkDetailsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkDetailsResponse>> Create([FromBody] CreateWorkRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _workService.CreateAsync(callerId, request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkDetailsResponse>> GetById([FromRoute] Guid id)
    {
        TryGetCallerId(out var callerId);
        var response = await _workService.GetByIdAsync(callerId, id);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<WorkCardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<WorkCardResponse>>> GetFeed([FromQuery] WorkListFilterRequest query)
    {
        var response = await _workService.GetPublishedFeedAsync(query);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkDetailsResponse>> Update([FromRoute] Guid id, [FromBody] UpdateWorkRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _workService.UpdateAsync(callerId, id, request);
        return Ok(response);
    }

    private bool TryGetCallerId(out Guid callerId)
    {
        var callerIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        return Guid.TryParse(callerIdRaw, out callerId);
    }
}
