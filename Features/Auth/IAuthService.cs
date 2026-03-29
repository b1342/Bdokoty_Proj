using System.Threading.Tasks;
using WorkshowcaseApi.Features.Auth.DTOs;

namespace WorkshowcaseApi.Features.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);
}
