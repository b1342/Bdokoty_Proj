using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Features.Works.DTOs.Responses;

public sealed class WorkDetailsResponse
{
    public Guid Id { get; set; }

    public Guid CreatedByUserId { get; set; }

    public string CreatedByType { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid PrimaryCategoryId { get; set; }

    public string PrimaryCategoryName { get; set; } = string.Empty;

    public string SpaceType { get; set; } = string.Empty;

    public DateTime? CompletionDate { get; set; }

    public bool HasBeforeAfter { get; set; }

    public bool IsAnonymous { get; set; }

    public List<WorkMediaResponse> Media { get; set; } = new();

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}
