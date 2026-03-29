using System;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Domain.Works;

public sealed class WorkProfessional
{
    public Guid Id { get; set; }

    public Guid WorkId { get; set; }

    public Work Work { get; set; } = null!;

    public Guid? ProfessionalUserId { get; set; }

    public User? ProfessionalUser { get; set; }

    public string? ExternalName { get; set; }

    public string? ProfessionCategory { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Whatsapp { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
