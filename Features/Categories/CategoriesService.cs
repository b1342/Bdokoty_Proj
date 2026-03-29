using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Domain.Categories;
using WorkshowcaseApi.Features.Categories.DTOs;

namespace WorkshowcaseApi.Features.Categories;

public sealed class CategoriesService : ICategoriesService
{
    private static readonly Regex MultiWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    private readonly ICategoryRepository _categoryRepository;

    public CategoriesService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllActiveAsync()
    {
        var categories = await _categoryRepository.GetAllActiveAsync();
        return categories.Select(ToCategoryResponse).ToArray();
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedDisplayName = NormalizeWhitespace(request.Name);
        var normalizedName = normalizedDisplayName.ToLowerInvariant();

        var existingCategory = await _categoryRepository.GetByNormalizedNameAsync(normalizedName);
        if (existingCategory is not null)
        {
            throw new ConflictException("Category with this name already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = normalizedDisplayName,
            NormalizedName = normalizedName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return ToCategoryResponse(category);
    }

    private static string NormalizeWhitespace(string rawValue)
    {
        var value = rawValue ?? string.Empty;
        return MultiWhitespaceRegex.Replace(value.Trim(), " ");
    }

    private static CategoryResponse ToCategoryResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
