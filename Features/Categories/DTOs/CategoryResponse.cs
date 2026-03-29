using System;

namespace WorkshowcaseApi.Features.Categories.DTOs;

public sealed class CategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
