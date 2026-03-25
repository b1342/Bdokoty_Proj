using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Domain.ProfessionalProfiles;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;
using WorkshowcaseApi.Features.Users;

namespace WorkshowcaseApi.Features.ProfessionalProfiles;

public sealed class ProfessionalProfilesService : IProfessionalProfilesService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IProfessionalProfileRepository _professionalProfileRepository;
    private readonly IUserRepository _userRepository;

    public ProfessionalProfilesService(
        IProfessionalProfileRepository professionalProfileRepository,
        IUserRepository userRepository)
    {
        _professionalProfileRepository = professionalProfileRepository ?? throw new ArgumentNullException(nameof(professionalProfileRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<ProfessionalProfileResponse> CreateAsync(Guid callerId, CreateProfessionalProfileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var caller = await GetProfessionalCallerOrThrowAsync(callerId);

        var existing = await _professionalProfileRepository.GetByUserIdAsync(caller.Id);
        if (existing is not null)
        {
            throw new ConflictException("Professional profile already exists for this user.");
        }

        var now = DateTime.UtcNow;
        var profile = new ProfessionalProfile
        {
            Id = Guid.NewGuid(),
            UserId = caller.Id,
            DisplayName = request.DisplayName.Trim(),
            Description = NormalizeNullable(request.Description),
            PrimaryCategory = request.PrimaryCategory.Trim(),
            SecondaryCategoriesJson = SerializeStringList(request.SecondaryCategories),
            ServiceAreasJson = SerializeStringList(request.ServiceAreas),
            ContactPhone = NormalizeNullable(request.ContactPhone),
            ContactEmail = NormalizeNullable(request.ContactEmail),
            WhatsappNumber = NormalizeNullable(request.WhatsappNumber),
            ContactPreference = request.ContactPreference.Trim().ToLowerInvariant(),
            LogoUrl = NormalizeNullable(request.LogoUrl),
            WebsiteUrl = NormalizeNullable(request.WebsiteUrl),
            IsPublic = request.IsPublic,
            IsVerified = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _professionalProfileRepository.AddAsync(profile);
        await _professionalProfileRepository.SaveChangesAsync();

        return ToProfessionalProfileResponse(profile);
    }

    public async Task<ProfessionalProfileResponse> GetMyProfileAsync(Guid callerId)
    {
        await GetProfessionalCallerOrThrowAsync(callerId);

        var profile = await _professionalProfileRepository.GetTrackedByUserIdAsync(callerId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        return ToProfessionalProfileResponse(profile);
    }

    public async Task<ProfessionalProfileResponse> UpdateMyProfileAsync(Guid callerId, UpdateProfessionalProfileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await GetProfessionalCallerOrThrowAsync(callerId);

        var profile = await _professionalProfileRepository.GetByUserIdAsync(callerId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        if (request.DisplayName is not null)
        {
            profile.DisplayName = request.DisplayName.Trim();
        }

        if (request.Description is not null)
        {
            profile.Description = NormalizeNullable(request.Description);
        }

        if (request.PrimaryCategory is not null)
        {
            profile.PrimaryCategory = request.PrimaryCategory.Trim();
        }

        if (request.SecondaryCategories is not null)
        {
            profile.SecondaryCategoriesJson = SerializeStringList(request.SecondaryCategories);
        }

        if (request.ServiceAreas is not null)
        {
            profile.ServiceAreasJson = SerializeStringList(request.ServiceAreas);
        }

        if (request.ContactPhone is not null)
        {
            profile.ContactPhone = NormalizeNullable(request.ContactPhone);
        }

        if (request.ContactEmail is not null)
        {
            profile.ContactEmail = NormalizeNullable(request.ContactEmail);
        }

        if (request.WhatsappNumber is not null)
        {
            profile.WhatsappNumber = NormalizeNullable(request.WhatsappNumber);
        }

        if (request.ContactPreference is not null)
        {
            profile.ContactPreference = request.ContactPreference.Trim().ToLowerInvariant();
        }

        if (request.LogoUrl is not null)
        {
            profile.LogoUrl = NormalizeNullable(request.LogoUrl);
        }

        if (request.WebsiteUrl is not null)
        {
            profile.WebsiteUrl = NormalizeNullable(request.WebsiteUrl);
        }

        if (request.IsPublic.HasValue)
        {
            profile.IsPublic = request.IsPublic.Value;
        }

        profile.UpdatedAt = DateTime.UtcNow;

        await _professionalProfileRepository.SaveChangesAsync();

        return ToProfessionalProfileResponse(profile);
    }

    public async Task<PublicProfessionalProfileResponse> GetPublicByIdAsync(Guid profileId)
    {
        var profile = await _professionalProfileRepository.GetPublicByIdAsync(profileId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        return ToPublicProfessionalProfileResponse(profile);
    }

    public async Task<PublicProfessionalProfileResponse> GetPublicByUserIdAsync(Guid userId)
    {
        var profile = await _professionalProfileRepository.GetPublicByUserIdAsync(userId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        return ToPublicProfessionalProfileResponse(profile);
    }

    public async Task<IReadOnlyList<ProfessionalProfileListItemResponse>> GetPublicListAsync()
    {
        var profiles = await _professionalProfileRepository.GetPublicListAsync();
        return profiles.Select(ToProfessionalProfileListItemResponse).ToArray();
    }

    private async Task<User> GetProfessionalCallerOrThrowAsync(Guid callerId)
    {
        var caller = await _userRepository.GetByIdAsync(callerId);
        if (caller is null || caller.Status == UserStatus.Deleted)
        {
            throw new NotFoundException("User was not found.");
        }

        if (caller.Status != UserStatus.Active)
        {
            throw new ForbiddenException("Only active users can manage professional profiles.");
        }

        if (caller.UserType != UserType.Professional)
        {
            throw new ForbiddenException("Only professional users can manage professional profiles.");
        }

        return caller;
    }

    private static string SerializeStringList(IReadOnlyCollection<string>? values)
    {
        var normalized = values?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToArray()
            ?? Array.Empty<string>();

        return JsonSerializer.Serialize(normalized, JsonOptions);
    }

    private static List<string> DeserializeStringList(string? json)
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

    private static string? NormalizeNullable(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    private static ProfessionalProfileResponse ToProfessionalProfileResponse(ProfessionalProfile profile)
    {
        return new ProfessionalProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            Description = profile.Description,
            PrimaryCategory = profile.PrimaryCategory,
            SecondaryCategories = DeserializeStringList(profile.SecondaryCategoriesJson),
            ServiceAreas = DeserializeStringList(profile.ServiceAreasJson),
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

    private static PublicProfessionalProfileResponse ToPublicProfessionalProfileResponse(ProfessionalProfile profile)
    {
        return new PublicProfessionalProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            Description = profile.Description,
            PrimaryCategory = profile.PrimaryCategory,
            SecondaryCategories = DeserializeStringList(profile.SecondaryCategoriesJson),
            ServiceAreas = DeserializeStringList(profile.ServiceAreasJson),
            ContactPhone = profile.ContactPhone,
            ContactEmail = profile.ContactEmail,
            WhatsappNumber = profile.WhatsappNumber,
            ContactPreference = profile.ContactPreference,
            LogoUrl = profile.LogoUrl,
            WebsiteUrl = profile.WebsiteUrl,
            IsVerified = profile.IsVerified
        };
    }

    private static ProfessionalProfileListItemResponse ToProfessionalProfileListItemResponse(ProfessionalProfile profile)
    {
        return new ProfessionalProfileListItemResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            PrimaryCategory = profile.PrimaryCategory,
            ServiceAreas = DeserializeStringList(profile.ServiceAreasJson),
            LogoUrl = profile.LogoUrl,
            IsVerified = profile.IsVerified
        };
    }
}
