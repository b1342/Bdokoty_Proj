using System;

namespace WorkshowcaseApi.Features.Auth.DTOs;

public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string UserType { get; set; } = string.Empty;
}
