using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Data;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Features.Works.Repositories;

public sealed class WorkRepository : IWorkRepository
{
    private readonly AppDbContext _dbContext;

    public WorkRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Work?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Works
            .Include(x => x.PrimaryCategory)
            .Include(x => x.Media)
            .Include(x => x.WorkTags).ThenInclude(wt => wt.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Work?> GetTrackedByIdAsync(Guid id)
    {
        return await _dbContext.Works
            .Include(x => x.PrimaryCategory)
            .Include(x => x.Media)
            .Include(x => x.WorkTags).ThenInclude(wt => wt.Tag)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Work>> GetPublishedAsync(
        Guid? primaryCategoryId,
        WorkSpaceType? spaceType,
        WorkCreatedByType? createdByType,
        int page,
        int pageSize)
    {
        var skip = (page - 1) * pageSize;

        var query = BuildPublishedQuery(primaryCategoryId, spaceType, createdByType);

        return await query
            .Include(x => x.PrimaryCategory)
            .AsNoTracking()
            .OrderByDescending(x => x.PublishedAt)
            .ThenBy(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountPublishedAsync(
        Guid? primaryCategoryId,
        WorkSpaceType? spaceType,
        WorkCreatedByType? createdByType)
    {
        return await BuildPublishedQuery(primaryCategoryId, spaceType, createdByType).CountAsync();
    }

    public async Task AddAsync(Work work)
    {
        await _dbContext.Works.AddAsync(work);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<Work> BuildPublishedQuery(
        Guid? primaryCategoryId,
        WorkSpaceType? spaceType,
        WorkCreatedByType? createdByType)
    {
        IQueryable<Work> query = _dbContext.Works
            .Where(x => x.Status == WorkStatus.Published);

        if (primaryCategoryId.HasValue)
        {
            query = query.Where(x => x.PrimaryCategoryId == primaryCategoryId.Value);
        }

        if (spaceType.HasValue)
        {
            query = query.Where(x => x.SpaceType == spaceType.Value);
        }

        if (createdByType.HasValue)
        {
            query = query.Where(x => x.CreatedByType == createdByType.Value);
        }

        return query;
    }
}
