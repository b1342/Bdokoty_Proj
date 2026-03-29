namespace WorkshowcaseApi.Features.Users.DTOs;

public sealed class UserListQuery
{
    public string? UserType { get; set; }

    public string? Status { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
