using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Requests;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Responses;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.Services;

public interface IProfessionalProfileService
{
    Task<ProfessionalProfileDetailsResponse> CreateAsync(Guid callerId, CreateProfessionalProfileRequest request);

    Task<ProfessionalProfileDetailsResponse> GetMyProfileAsync(Guid callerId);

    Task<ProfessionalProfileDetailsResponse> UpdateMyProfileAsync(Guid callerId, UpdateProfessionalProfileRequest request);

    Task<PublicProfessionalProfileResponse> GetPublicByIdAsync(Guid profileId);

    Task<PublicProfessionalProfileResponse> GetPublicByUserIdAsync(Guid userId);

    Task<IReadOnlyList<ProfessionalProfileCardResponse>> GetPublicListAsync();
}
