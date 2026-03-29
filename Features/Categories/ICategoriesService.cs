using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.Categories.DTOs;

namespace WorkshowcaseApi.Features.Categories;

public interface ICategoriesService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllActiveAsync();

    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
}
