using System;

namespace WorkshowcaseApi.Features.Works.DTOs;

public sealed class WorkListItemResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid PrimaryCategoryId { get; set; }

    public string PrimaryCategoryName { get; set; } = string.Empty;

    public string SpaceType { get; set; } = string.Empty;

    public DateTime? CompletionDate { get; set; }

    public bool HasBeforeAfter { get; set; }

    public bool IsAnonymous { get; set; }

    public string CreatedByType { get; set; } = string.Empty;

    public DateTime? PublishedAt { get; set; }
}
