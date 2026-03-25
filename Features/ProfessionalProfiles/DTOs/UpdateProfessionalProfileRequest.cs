using System.Collections.Generic;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;

public sealed class UpdateProfessionalProfileRequest
{
    public string? DisplayName { get; set; }

    public string? Description { get; set; }

    public string? PrimaryCategory { get; set; }

    public List<string>? SecondaryCategories { get; set; }

    public List<string>? ServiceAreas { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? ContactPreference { get; set; }

    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public bool? IsPublic { get; set; }
}
