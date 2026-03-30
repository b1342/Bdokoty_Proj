using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Features.Auth.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
