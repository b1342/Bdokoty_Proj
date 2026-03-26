using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Data;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Features.Works;

public sealed class WorkMediaRepository : IWorkMediaRepository
{
    private readonly AppDbContext _dbContext;

    public WorkMediaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<WorkMedia?> GetByIdAsync(Guid id)
    {
        return await _dbContext.WorkMedia
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<WorkMedia?> GetTrackedByIdAsync(Guid id)
    {
        return await _dbContext.WorkMedia
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<WorkMedia>> GetByWorkIdAsync(Guid workId)
    {
        return await _dbContext.WorkMedia
            .AsNoTracking()
            .Where(x => x.WorkId == workId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(WorkMedia media)
    {
        await _dbContext.WorkMedia.AddAsync(media);
    }

    public void Remove(WorkMedia media)
    {
        _dbContext.WorkMedia.Remove(media);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
