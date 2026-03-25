using System;

namespace WorkshowcaseApi.Domain.ProfessionalProfiles;

public sealed class ProfessionalProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid PrimaryCategoryId { get; set; }

    public string SecondaryCategoriesJson { get; set; } = "[]";

    public string ServiceAreasJson { get; set; } = "[]";

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? WhatsappNumber { get; set; }

    public string ContactPreference { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public bool IsPublic { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
