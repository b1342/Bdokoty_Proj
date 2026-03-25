using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;

public sealed class PublicProfessionalProfileResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string PrimaryCategory { get; set; } = string.Empty;

    public List<string> SecondaryCategories { get; set; } = new List<string>();

    public List<string> ServiceAreas { get; set; } = new List<string>();

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? WhatsappNumber { get; set; }

    public string ContactPreference { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public bool IsVerified { get; set; }
}
