using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;

namespace WorkshowcaseApi.Features.ProfessionalProfiles;

[ApiController]
[Route("api/professional-profiles")]
public sealed class ProfessionalProfilesController : ControllerBase
{
    private readonly IProfessionalProfilesService _professionalProfilesService;

    public ProfessionalProfilesController(IProfessionalProfilesService professionalProfilesService)
    {
        _professionalProfilesService = professionalProfilesService ?? throw new ArgumentNullException(nameof(professionalProfilesService));
    }

    [Authorize(Roles = "Professional")]
    [HttpPost]
    [ProducesResponseType(typeof(ProfessionalProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProfessionalProfileResponse>> Create([FromBody] CreateProfessionalProfileRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _professionalProfilesService.CreateAsync(callerId, request);
        return CreatedAtAction(nameof(GetPublicById), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Professional")]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ProfessionalProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProfessionalProfileResponse>> GetMyProfile()
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _professionalProfilesService.GetMyProfileAsync(callerId);
        return Ok(response);
    }

    [Authorize(Roles = "Professional")]
    [HttpPut("me")]
    [ProducesResponseType(typeof(ProfessionalProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProfessionalProfileResponse>> UpdateMyProfile([FromBody] UpdateProfessionalProfileRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _professionalProfilesService.UpdateMyProfileAsync(callerId, request);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PublicProfessionalProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicProfessionalProfileResponse>> GetPublicById([FromRoute] Guid id)
    {
        var response = await _professionalProfilesService.GetPublicByIdAsync(id);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(PublicProfessionalProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicProfessionalProfileResponse>> GetPublicByUserId([FromRoute] Guid userId)
    {
        var response = await _professionalProfilesService.GetPublicByUserIdAsync(userId);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProfessionalProfileListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProfessionalProfileListItemResponse>>> GetPublicList()
    {
        var response = await _professionalProfilesService.GetPublicListAsync();
        return Ok(response);
    }

    private bool TryGetCallerId(out Guid callerId)
    {
        var callerIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        return Guid.TryParse(callerIdRaw, out callerId);
    }
}
