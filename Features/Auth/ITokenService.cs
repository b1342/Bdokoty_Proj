using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Features.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}
