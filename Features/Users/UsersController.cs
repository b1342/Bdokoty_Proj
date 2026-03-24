using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Users.DTOs;

namespace WorkshowcaseApi.Features.Users;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    [Authorize]
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateMeRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _usersService.UpdateMeAsync(callerId, request);
        return Ok(response);
    }

    [Authorize]
    [HttpPatch("me/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        await _usersService.ChangePasswordAsync(callerId, request);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResponse<UserSummaryResponse>>> GetAll([FromQuery] UserListQuery query)
    {
        var response = await _usersService.GetAllAsync(query);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserResponse>> GetById([FromRoute] Guid id)
    {
        var response = await _usersService.GetByIdAsync(id);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserResponse>> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateStatusRequest request)
    {
        if (!TryGetCallerId(out var adminId))
        {
            return Unauthorized();
        }

        var response = await _usersService.UpdateStatusAsync(adminId, id, request);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        if (!TryGetCallerId(out var adminId))
        {
            return Unauthorized();
        }

        await _usersService.DeleteAsync(adminId, id);
        return NoContent();
    }

    private bool TryGetCallerId(out Guid callerId)
    {
        var callerIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        return Guid.TryParse(callerIdRaw, out callerId);
    }
}
