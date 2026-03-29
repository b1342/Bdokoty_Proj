using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Requests;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Responses;
using WorkshowcaseApi.Features.ProfessionalProfiles.Services;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.Controllers;

[ApiController]
[Route("api/professional-profiles")]
public sealed class ProfessionalProfilesController : ControllerBase
{
    private readonly IProfessionalProfileService _professionalProfileService;

    public ProfessionalProfilesController(IProfessionalProfileService professionalProfileService)
    {
        _professionalProfileService = professionalProfileService ?? throw new ArgumentNullException(nameof(professionalProfileService));
    }

    [Authorize(Roles = "Professional")]
    [HttpPost]
    [ProducesResponseType(typeof(ProfessionalProfileDetailsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProfessionalProfileDetailsResponse>> Create([FromBody] CreateProfessionalProfileRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _professionalProfileService.CreateAsync(callerId, request);
        return CreatedAtAction(nameof(GetPublicById), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Professional")]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ProfessionalProfileDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProfessionalProfileDetailsResponse>> GetMyProfile()
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _professionalProfileService.GetMyProfileAsync(callerId);
        return Ok(response);
    }

    [Authorize(Roles = "Professional")]
    [HttpPut("me")]
    [ProducesResponseType(typeof(ProfessionalProfileDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProfessionalProfileDetailsResponse>> UpdateMyProfile([FromBody] UpdateProfessionalProfileRequest request)
    {
        if (!TryGetCallerId(out var callerId))
        {
            return Unauthorized();
        }

        var response = await _professionalProfileService.UpdateMyProfileAsync(callerId, request);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PublicProfessionalProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicProfessionalProfileResponse>> GetPublicById([FromRoute] Guid id)
    {
        var response = await _professionalProfileService.GetPublicByIdAsync(id);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(PublicProfessionalProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicProfessionalProfileResponse>> GetPublicByUserId([FromRoute] Guid userId)
    {
        var response = await _professionalProfileService.GetPublicByUserIdAsync(userId);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProfessionalProfileCardResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProfessionalProfileCardResponse>>> GetPublicList()
    {
        var response = await _professionalProfileService.GetPublicListAsync();
        return Ok(response);
    }

    private bool TryGetCallerId(out Guid callerId)
    {
        var callerIdRaw = User.FindFirstValue(AppClaimTypes.UserId);
        return Guid.TryParse(callerIdRaw, out callerId);
    }
}
