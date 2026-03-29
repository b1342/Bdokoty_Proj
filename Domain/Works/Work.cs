using System;
using System.Collections.Generic;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Domain.Categories;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Domain.Works;

public sealed class Work
{
    public Guid Id { get; set; }

    public Guid CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public Guid PrimaryCategoryId { get; set; }

    public Category PrimaryCategory { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? CompletionDate { get; set; }

    public WorkSpaceType SpaceType { get; set; }

    public WorkCreatedByType CreatedByType { get; set; }

    public WorkStatus Status { get; set; }

    public bool HasBeforeAfter { get; set; }

    public bool IsAnonymous { get; set; }

    public ICollection<WorkMedia> Media { get; set; } = new List<WorkMedia>();

    public ICollection<WorkProfessional> Professionals { get; set; } = new List<WorkProfessional>();
    public ICollection<WorkTag> WorkTags { get; set; } = new List<WorkTag>();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}
