using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Domain.ProfessionalProfiles;

namespace WorkshowcaseApi.Features.ProfessionalProfiles;

public interface IProfessionalProfileRepository
{
    Task<ProfessionalProfile?> GetByIdAsync(Guid id);

    Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId);

    Task<ProfessionalProfile?> GetTrackedByUserIdAsync(Guid userId);

    Task<ProfessionalProfile?> GetPublicByIdAsync(Guid id);

    Task<ProfessionalProfile?> GetPublicByUserIdAsync(Guid userId);

    Task<IReadOnlyList<ProfessionalProfile>> GetPublicListAsync();

    Task AddAsync(ProfessionalProfile profile);

    void Update(ProfessionalProfile profile);

    Task SaveChangesAsync();
}
