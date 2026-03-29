using System;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Features.Auth.DTOs.Requests;
using WorkshowcaseApi.Features.Auth.DTOs.Responses;
using WorkshowcaseApi.Features.Users.Repositories;

namespace WorkshowcaseApi.Features.Auth.Services;

public sealed class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedEmail = NormalizeEmail(request.Email);
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingUser is not null)
        {
            throw new ConflictException("Email is already registered.");
        }

        var requestedUserType = ParseUserType(request.UserType);
        if (requestedUserType == UserType.Admin)
        {
            throw new ValidationException("Admin role cannot be selected during registration.");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = NormalizeName(request.FullName),
            Email = normalizedEmail,
            Phone = NormalizeOptional(request.Phone),
            PasswordHash = BCryptNet.HashPassword(request.Password),
            UserType = requestedUserType,
            ProfileImageUrl = null,
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);
        return ToAuthResponse(user, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (user is null)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        if (user.Status == UserStatus.Deleted)
        {
            // Do not reveal whether the account exists.
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        if (user.Status == UserStatus.Blocked)
        {
            throw new ForbiddenException("Your account has been blocked.");
        }

        var isValidPassword = BCryptNet.Verify(request.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var token = _tokenService.GenerateToken(user);
        return ToAuthResponse(user, token);
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.Status == UserStatus.Deleted)
        {
            throw new NotFoundException("User was not found.");
        }

        if (user.Status != UserStatus.Active)
        {
            throw new ForbiddenException("Only active users can access this endpoint.");
        }

        return new CurrentUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            UserType = user.UserType.ToString(),
            ProfileImageUrl = user.ProfileImageUrl,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    private static AuthResponse ToAuthResponse(User user, string token)
    {
        return new AuthResponse
        {
            Token = token,
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType.ToString()
        };
    }

    private static string NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return string.Empty;
        }

        return email.Trim().ToLowerInvariant();
    }

    private static string NormalizeName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return string.Empty;
        }

        return fullName.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private static UserType ParseUserType(string? rawUserType)
    {
        if (Enum.TryParse<UserType>(rawUserType, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new ValidationException("Invalid user type.");
    }
}
