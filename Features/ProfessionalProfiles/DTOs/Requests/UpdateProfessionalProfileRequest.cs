using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Requests;

public sealed class UpdateProfessionalProfileRequest
{
    public string? DisplayName { get; set; }

    public string? Description { get; set; }

    public Guid? PrimaryCategoryId { get; set; }

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
