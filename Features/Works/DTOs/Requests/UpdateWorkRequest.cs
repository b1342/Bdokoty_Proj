using System;

namespace WorkshowcaseApi.Features.Works.DTOs.Requests;

public sealed class UpdateWorkRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public Guid? PrimaryCategoryId { get; set; }

    public string? SpaceType { get; set; }

    public DateTime? CompletionDate { get; set; }

    public bool? HasBeforeAfter { get; set; }

    public bool? IsAnonymous { get; set; }

    public string? Status { get; set; }
}
