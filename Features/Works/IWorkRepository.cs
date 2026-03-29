using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Features.Works;

public interface IWorkRepository
{
    Task<Work?> GetByIdAsync(Guid id);

    Task<Work?> GetTrackedByIdAsync(Guid id);

    Task<IReadOnlyList<Work>> GetPublishedAsync(
        Guid? primaryCategoryId,
        WorkSpaceType? spaceType,
        WorkCreatedByType? createdByType,
        int page,
        int pageSize);

    Task<int> CountPublishedAsync(
        Guid? primaryCategoryId,
        WorkSpaceType? spaceType,
        WorkCreatedByType? createdByType);

    Task AddAsync(Work work);

    Task SaveChangesAsync();
}
