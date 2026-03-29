using System;

namespace WorkshowcaseApi.Features.Categories.DTOs.Responses;

public sealed class CategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
