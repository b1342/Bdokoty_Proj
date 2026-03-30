using System;

namespace WorkshowcaseApi.Domain.Works;

public sealed class Tag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
