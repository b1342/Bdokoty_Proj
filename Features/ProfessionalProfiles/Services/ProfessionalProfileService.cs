using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Domain.Categories;
using WorkshowcaseApi.Domain.ProfessionalProfiles;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Features.Categories.Repositories;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Requests;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Responses;
using WorkshowcaseApi.Features.ProfessionalProfiles.Mappings;
using WorkshowcaseApi.Features.ProfessionalProfiles.Repositories;
using WorkshowcaseApi.Features.Users.Repositories;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.Services;

public sealed class ProfessionalProfileService : IProfessionalProfileService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProfessionalProfileRepository _professionalProfileRepository;
    private readonly IUserRepository _userRepository;

    public ProfessionalProfileService(
        ICategoryRepository categoryRepository,
        IProfessionalProfileRepository professionalProfileRepository,
        IUserRepository userRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _professionalProfileRepository = professionalProfileRepository ?? throw new ArgumentNullException(nameof(professionalProfileRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<ProfessionalProfileDetailsResponse> CreateAsync(Guid callerId, CreateProfessionalProfileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var caller = await GetProfessionalCallerOrThrowAsync(callerId);

        var existing = await _professionalProfileRepository.GetByUserIdAsync(caller.Id);
        if (existing is not null)
        {
            throw new ConflictException("Professional profile already exists for this user.");
        }

        var category = await GetActiveCategoryOrThrowAsync(request.PrimaryCategoryId);
        var now = DateTime.UtcNow;
        var profile = new ProfessionalProfile
        {
            Id = Guid.NewGuid(),
            UserId = caller.Id,
            DisplayName = request.DisplayName.Trim(),
            Description = NormalizeNullable(request.Description),
            PrimaryCategoryId = category.Id,
            SecondaryCategoriesJson = ProfessionalProfileDtoMapper.SerializeStringList(request.SecondaryCategories),
            ServiceAreasJson = ProfessionalProfileDtoMapper.SerializeStringList(request.ServiceAreas),
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

        return await ToProfessionalProfileDetailsResponseAsync(profile);
    }

    public async Task<ProfessionalProfileDetailsResponse> GetMyProfileAsync(Guid callerId)
    {
        await GetProfessionalCallerOrThrowAsync(callerId);

        var profile = await _professionalProfileRepository.GetTrackedByUserIdAsync(callerId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        return await ToProfessionalProfileDetailsResponseAsync(profile);
    }

    public async Task<ProfessionalProfileDetailsResponse> UpdateMyProfileAsync(Guid callerId, UpdateProfessionalProfileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await GetProfessionalCallerOrThrowAsync(callerId);

        var profile = await _professionalProfileRepository.GetTrackedByUserIdAsync(callerId);
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

        if (request.PrimaryCategoryId.HasValue)
        {
            var category = await GetActiveCategoryOrThrowAsync(request.PrimaryCategoryId.Value);
            profile.PrimaryCategoryId = category.Id;
        }

        if (request.SecondaryCategories is not null)
        {
            profile.SecondaryCategoriesJson = ProfessionalProfileDtoMapper.SerializeStringList(request.SecondaryCategories);
        }

        if (request.ServiceAreas is not null)
        {
            profile.ServiceAreasJson = ProfessionalProfileDtoMapper.SerializeStringList(request.ServiceAreas);
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

        return await ToProfessionalProfileDetailsResponseAsync(profile);
    }

    public async Task<PublicProfessionalProfileResponse> GetPublicByIdAsync(Guid profileId)
    {
        var profile = await _professionalProfileRepository.GetPublicByIdAsync(profileId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        return await ToPublicProfessionalProfileResponseAsync(profile);
    }

    public async Task<PublicProfessionalProfileResponse> GetPublicByUserIdAsync(Guid userId)
    {
        var profile = await _professionalProfileRepository.GetPublicByUserIdAsync(userId);
        if (profile is null)
        {
            throw new NotFoundException("Professional profile was not found.");
        }

        return await ToPublicProfessionalProfileResponseAsync(profile);
    }

    public async Task<IReadOnlyList<ProfessionalProfileCardResponse>> GetPublicListAsync()
    {
        var profiles = await _professionalProfileRepository.GetPublicListAsync();
        var responses = new List<ProfessionalProfileCardResponse>(profiles.Count);
        foreach (var profile in profiles)
        {
            responses.Add(await ToProfessionalProfileCardResponseAsync(profile));
        }

        return responses;
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

    private static string? NormalizeNullable(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    private async Task<Category> GetActiveCategoryOrThrowAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null || !category.IsActive)
        {
            throw new ValidationException("PrimaryCategoryId is invalid.");
        }

        return category;
    }

    private async Task<string> GetPrimaryCategoryNameAsync(Guid primaryCategoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(primaryCategoryId);
        return category?.Name ?? string.Empty;
    }

    private async Task<ProfessionalProfileDetailsResponse> ToProfessionalProfileDetailsResponseAsync(ProfessionalProfile profile)
    {
        var primaryCategoryName = await GetPrimaryCategoryNameAsync(profile.PrimaryCategoryId);

        return ProfessionalProfileDtoMapper.ToDetailsResponse(
            profile,
            primaryCategoryName,
            ProfessionalProfileDtoMapper.DeserializeStringList(profile.SecondaryCategoriesJson),
            ProfessionalProfileDtoMapper.DeserializeStringList(profile.ServiceAreasJson));
    }

    private async Task<PublicProfessionalProfileResponse> ToPublicProfessionalProfileResponseAsync(ProfessionalProfile profile)
    {
        var primaryCategoryName = await GetPrimaryCategoryNameAsync(profile.PrimaryCategoryId);

        return ProfessionalProfileDtoMapper.ToPublicResponse(
            profile,
            primaryCategoryName,
            ProfessionalProfileDtoMapper.DeserializeStringList(profile.SecondaryCategoriesJson),
            ProfessionalProfileDtoMapper.DeserializeStringList(profile.ServiceAreasJson));
    }

    private async Task<ProfessionalProfileCardResponse> ToProfessionalProfileCardResponseAsync(ProfessionalProfile profile)
    {
        var primaryCategoryName = await GetPrimaryCategoryNameAsync(profile.PrimaryCategoryId);

        return ProfessionalProfileDtoMapper.ToCardResponse(
            profile,
            primaryCategoryName,
            ProfessionalProfileDtoMapper.DeserializeStringList(profile.ServiceAreasJson));
    }
}
