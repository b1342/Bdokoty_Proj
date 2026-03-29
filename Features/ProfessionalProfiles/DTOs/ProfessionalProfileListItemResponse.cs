using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;

public sealed class ProfessionalProfileListItemResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string PrimaryCategoryName { get; set; } = string.Empty;

    public List<string> ServiceAreas { get; set; } = new List<string>();

    public string? LogoUrl { get; set; }

    public bool IsVerified { get; set; }
}
