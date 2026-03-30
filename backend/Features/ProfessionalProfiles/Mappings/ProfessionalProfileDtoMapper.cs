using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WorkshowcaseApi.Domain.ProfessionalProfiles;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Responses;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.Mappings;

public static class ProfessionalProfileDtoMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static string SerializeStringList(IReadOnlyCollection<string>? values)
    {
        var normalized = values?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToArray()
            ?? Array.Empty<string>();

        return JsonSerializer.Serialize(normalized, JsonOptions);
    }

    public static List<string> DeserializeStringList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? new List<string>();
        }
        catch (JsonException)
        {
            return new List<string>();
        }
    }

    public static ProfessionalProfileDetailsResponse ToDetailsResponse(
        ProfessionalProfile profile,
        string primaryCategoryName,
        List<string> secondaryCategories,
        List<string> serviceAreas)
    {
        return new ProfessionalProfileDetailsResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            Description = profile.Description,
            PrimaryCategoryId = profile.PrimaryCategoryId,
            PrimaryCategoryName = primaryCategoryName,
            SecondaryCategories = secondaryCategories,
            ServiceAreas = serviceAreas,
            ContactPhone = profile.ContactPhone,
            ContactEmail = profile.ContactEmail,
            WhatsappNumber = profile.WhatsappNumber,
            ContactPreference = profile.ContactPreference,
            LogoUrl = profile.LogoUrl,
            WebsiteUrl = profile.WebsiteUrl,
            IsPublic = profile.IsPublic,
            IsVerified = profile.IsVerified,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }

    public static PublicProfessionalProfileResponse ToPublicResponse(
        ProfessionalProfile profile,
        string primaryCategoryName,
        List<string> secondaryCategories,
        List<string> serviceAreas)
    {
        return new PublicProfessionalProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            Description = profile.Description,
            PrimaryCategoryId = profile.PrimaryCategoryId,
            PrimaryCategoryName = primaryCategoryName,
            SecondaryCategories = secondaryCategories,
            ServiceAreas = serviceAreas,
            ContactPhone = profile.ContactPhone,
            ContactEmail = profile.ContactEmail,
            WhatsappNumber = profile.WhatsappNumber,
            ContactPreference = profile.ContactPreference,
            LogoUrl = profile.LogoUrl,
            WebsiteUrl = profile.WebsiteUrl,
            IsVerified = profile.IsVerified
        };
    }

    public static ProfessionalProfileCardResponse ToCardResponse(
        ProfessionalProfile profile,
        string primaryCategoryName,
        List<string> serviceAreas)
    {
        return new ProfessionalProfileCardResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            PrimaryCategoryName = primaryCategoryName,
            ServiceAreas = serviceAreas,
            LogoUrl = profile.LogoUrl,
            IsVerified = profile.IsVerified
        };
    }
}
