using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs;

namespace WorkshowcaseApi.Features.ProfessionalProfiles;

public interface IProfessionalProfilesService
{
    Task<ProfessionalProfileResponse> CreateAsync(Guid callerId, CreateProfessionalProfileRequest request);

    Task<ProfessionalProfileResponse> GetMyProfileAsync(Guid callerId);

    Task<ProfessionalProfileResponse> UpdateMyProfileAsync(Guid callerId, UpdateProfessionalProfileRequest request);

    Task<PublicProfessionalProfileResponse> GetPublicByIdAsync(Guid profileId);

    Task<PublicProfessionalProfileResponse> GetPublicByUserIdAsync(Guid userId);

    Task<IReadOnlyList<ProfessionalProfileListItemResponse>> GetPublicListAsync();
}
