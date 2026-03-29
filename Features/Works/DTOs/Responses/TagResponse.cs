using System;

namespace WorkshowcaseApi.Features.Works.DTOs.Responses;

public sealed class TagResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
