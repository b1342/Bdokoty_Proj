using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.Auth.DTOs.Requests;
using WorkshowcaseApi.Features.Auth.DTOs.Responses;

namespace WorkshowcaseApi.Features.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId);
}
