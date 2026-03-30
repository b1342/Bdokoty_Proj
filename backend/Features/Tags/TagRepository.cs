using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Data;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Features.Tags;

public sealed class TagRepository : ITagRepository
{
    private readonly AppDbContext _dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        return await _dbContext.Tags
            .Where(t => idList.Contains(t.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Tag>> GetAllAsync()
    {
        return await _dbContext.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetByNormalizedNameAsync(string normalizedName)
    {
        return await _dbContext.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName);
    }

    public async Task AddAsync(Tag tag)
    {
        await _dbContext.Tags.AddAsync(tag);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
