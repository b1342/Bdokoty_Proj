using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Features.Works.Repositories;

public interface IWorkMediaRepository
{
    Task<WorkMedia?> GetByIdAsync(Guid id);

    Task<WorkMedia?> GetTrackedByIdAsync(Guid id);

    Task<List<WorkMedia>> GetByWorkIdAsync(Guid workId);

    Task AddAsync(WorkMedia media);

    void Remove(WorkMedia media);

    Task SaveChangesAsync();
}
