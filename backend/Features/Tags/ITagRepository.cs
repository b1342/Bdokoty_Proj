using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Domain.Works;

namespace WorkshowcaseApi.Features.Tags;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<IReadOnlyList<Tag>> GetAllAsync();

    Task<Tag?> GetByNormalizedNameAsync(string normalizedName);

    Task AddAsync(Tag tag);

    Task SaveChangesAsync();
}
