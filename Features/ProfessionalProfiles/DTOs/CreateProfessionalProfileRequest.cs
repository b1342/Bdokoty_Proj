using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;

public sealed class CreateProfessionalProfileRequest
{
    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid PrimaryCategoryId { get; set; }

    public List<string> SecondaryCategories { get; set; } = new();

    public List<string> ServiceAreas { get; set; } = new();

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? WhatsappNumber { get; set; }

    public string ContactPreference { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public bool IsPublic { get; set; }
}
