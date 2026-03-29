using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Data;
using WorkshowcaseApi.Domain.ProfessionalProfiles;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.Repositories;

public sealed class ProfessionalProfileRepository : IProfessionalProfileRepository
{
    private readonly AppDbContext _dbContext;

    public ProfessionalProfileRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ProfessionalProfile?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<ProfessionalProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Set<ProfessionalProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<ProfessionalProfile?> GetTrackedByUserIdAsync(Guid userId)
    {
        return await _dbContext.Set<ProfessionalProfile>()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<ProfessionalProfile?> GetPublicByIdAsync(Guid id)
    {
        return await _dbContext.Set<ProfessionalProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.IsPublic);
    }

    public async Task<ProfessionalProfile?> GetPublicByUserIdAsync(Guid userId)
    {
        return await _dbContext.Set<ProfessionalProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsPublic);
    }

    public async Task<IReadOnlyList<ProfessionalProfile>> GetPublicListAsync()
    {
        return await _dbContext.Set<ProfessionalProfile>()
            .AsNoTracking()
            .Where(x => x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(ProfessionalProfile profile)
    {
        await _dbContext.Set<ProfessionalProfile>().AddAsync(profile);
    }

    public void Update(ProfessionalProfile profile)
    {
        _dbContext.Set<ProfessionalProfile>().Update(profile);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
