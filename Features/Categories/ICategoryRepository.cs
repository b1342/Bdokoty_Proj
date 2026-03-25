using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Domain.Categories;

namespace WorkshowcaseApi.Features.Categories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllActiveAsync();

    Task<Category?> GetByNormalizedNameAsync(string normalizedName);

    Task<Category?> GetByIdAsync(Guid id);

    Task AddAsync(Category category);

    Task SaveChangesAsync();
}
