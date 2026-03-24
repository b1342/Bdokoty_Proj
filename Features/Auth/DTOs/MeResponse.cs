using System;

namespace WorkshowcaseApi.Features.Auth.DTOs;

public sealed class MeResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string UserType { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
