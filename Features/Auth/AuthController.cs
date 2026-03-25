using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Features.Auth.DTOs;
using WorkshowcaseApi.Features.Users;

namespace WorkshowcaseApi.Features.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AuthController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MeResponse>> Me()
    {
        var userIdRaw = User.FindFirstValue(AppClaimTypes.UserId);

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.Status == UserStatus.Deleted)
        {
            throw new NotFoundException("User was not found.");
        }

        if (user.Status != UserStatus.Active)
        {
            throw new ForbiddenException("Only active users can access this endpoint.");
        }

        var response = new MeResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            UserType = user.UserType.ToString(),
            ProfileImageUrl = user.ProfileImageUrl,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt
        };

        return Ok(response);
    }
}
