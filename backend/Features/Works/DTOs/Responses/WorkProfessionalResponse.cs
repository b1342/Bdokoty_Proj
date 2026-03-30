using System;

namespace WorkshowcaseApi.Features.Works.DTOs.Responses;

public sealed class WorkProfessionalResponse
{
    public Guid? ProfessionalUserId { get; set; }

    public string? DisplayName { get; set; }

    public string? ProfessionCategory { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Whatsapp { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}
