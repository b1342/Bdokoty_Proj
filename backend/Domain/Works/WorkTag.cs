using System;

namespace WorkshowcaseApi.Domain.Works;

public sealed class WorkTag
{
    public Guid WorkId { get; set; }

    public Work Work { get; set; } = null!;

    public Guid TagId { get; set; }

    public Tag Tag { get; set; } = null!;
}
