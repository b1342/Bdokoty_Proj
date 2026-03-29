using System;
using System.Collections.Generic;

namespace WorkshowcaseApi.Features.Works.DTOs.Requests;

public sealed class CreateWorkRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid PrimaryCategoryId { get; set; }

    public string SpaceType { get; set; } = string.Empty;

    public DateTime? CompletionDate { get; set; }

    public bool HasBeforeAfter { get; set; }

    public bool IsAnonymous { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<WorkProfessionalItemRequest> Professionals { get; set; } = new();
}
