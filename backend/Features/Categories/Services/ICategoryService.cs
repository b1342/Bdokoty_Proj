using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.Categories.DTOs.Requests;
using WorkshowcaseApi.Features.Categories.DTOs.Responses;

namespace WorkshowcaseApi.Features.Categories.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllActiveAsync();

    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
}
