using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Features.Auth.DTOs;

namespace WorkshowcaseApi.Features.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
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
    public ActionResult<MeResponse> Me()
    {
        var userIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        var email = User.FindFirstValue(AppClaimTypes.Email);
        var fullName = User.FindFirstValue(AppClaimTypes.FullName);
        var userType = User.FindFirstValue(AppClaimTypes.Role);
        var status = User.FindFirstValue(AppClaimTypes.Status);
        var createdAtRaw = User.FindFirstValue(AppClaimTypes.CreatedAt);

        if (!Guid.TryParse(userIdRaw, out var userId) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(userType) ||
            string.IsNullOrWhiteSpace(status))
        {
            return Unauthorized();
        }

        DateTime createdAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(createdAtRaw) &&
            DateTime.TryParse(
                createdAtRaw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var parsedCreatedAt))
        {
            createdAt = parsedCreatedAt;
        }

        var response = new MeResponse
        {
            Id = userId,
            Email = email,
            FullName = fullName,
            Phone = User.FindFirstValue(AppClaimTypes.Phone),
            UserType = userType,
            ProfileImageUrl = User.FindFirstValue(AppClaimTypes.ProfileImageUrl),
            Status = status,
            CreatedAt = createdAt
        };

        return Ok(response);
    }
}
