using System;
using System.Linq;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Features.Users.DTOs;

namespace WorkshowcaseApi.Features.Users;

public sealed class UsersService : IUsersService
{
    private readonly IUserRepository _userRepository;

    public UsersService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserResponse> UpdateMeAsync(Guid callerId, UpdateMeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await GetUserOrThrowAsync(callerId);

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            user.Phone = request.Phone.Trim();
        }

        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return ToUserResponse(user);
    }

    public async Task ChangePasswordAsync(Guid callerId, ChangePasswordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await GetUserOrThrowAsync(callerId);
        if (user.Status == UserStatus.Blocked)
        {
            throw new ForbiddenException("Your account is blocked.");
        }

        var isCurrentPasswordValid = BCryptNet.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            throw new UnauthorizedException("Current password is incorrect.");
        }

        user.PasswordHash = BCryptNet.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<PagedResponse<UserSummaryResponse>> GetAllAsync(UserListQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;

        var parsedUserType = ParseUserType(query.UserType);
        var parsedStatus = ParseUserStatus(query.Status);

        var users = await _userRepository.GetAllAsync(parsedUserType, parsedStatus, page, pageSize);
        var totalCount = await _userRepository.CountAsync(parsedUserType, parsedStatus);

        var mappedItems = users
            .Select(ToUserSummaryResponse)
            .ToArray();

        return new PagedResponse<UserSummaryResponse>(mappedItems, totalCount, page, pageSize);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await GetUserOrThrowAsync(id);
        return ToUserResponse(user);
    }

    public async Task<UserResponse> UpdateStatusAsync(Guid adminId, Guid targetId, UpdateStatusRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (adminId == targetId)
        {
            throw new ForbiddenException("You cannot change your own status.");
        }

        var targetUser = await GetUserOrThrowAsync(targetId);
        var parsedStatus = ParseAllowedStatus(request.Status);

        targetUser.Status = parsedStatus;
        targetUser.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(targetUser);
        await _userRepository.SaveChangesAsync();

        return ToUserResponse(targetUser);
    }

    public async Task DeleteAsync(Guid adminId, Guid targetId)
    {
        if (adminId == targetId)
        {
            throw new ForbiddenException("You cannot delete your own account.");
        }

        var targetUser = await GetUserOrThrowAsync(targetId);

        targetUser.Status = UserStatus.Deleted;
        targetUser.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(targetUser);
        await _userRepository.SaveChangesAsync();
    }

    private async Task<User> GetUserOrThrowAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null || user.Status == UserStatus.Deleted)
        {
            throw new NotFoundException("User was not found.");
        }

        return user;
    }

    private static UserType? ParseUserType(string? rawUserType)
    {
        if (string.IsNullOrWhiteSpace(rawUserType))
        {
            return null;
        }

        return Enum.TryParse<UserType>(rawUserType.Trim(), ignoreCase: true, out var parsed)
            ? parsed
            : null;
    }

    private static UserStatus? ParseUserStatus(string? rawStatus)
    {
        if (string.IsNullOrWhiteSpace(rawStatus))
        {
            return null;
        }

        return Enum.TryParse<UserStatus>(rawStatus.Trim(), ignoreCase: true, out var parsed)
            ? parsed
            : null;
    }

    private static UserStatus ParseAllowedStatus(string? rawStatus)
    {
        if (!Enum.TryParse<UserStatus>(rawStatus?.Trim(), ignoreCase: true, out var parsed))
        {
            throw new ValidationException("Invalid status value.");
        }

        return parsed switch
        {
            UserStatus.Active => UserStatus.Active,
            UserStatus.Pending => UserStatus.Pending,
            UserStatus.Blocked => UserStatus.Blocked,
            _ => throw new ValidationException("Only Active, Pending, and Blocked are allowed.")
        };
    }

    private static UserResponse ToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            UserType = user.UserType.ToString(),
            ProfileImageUrl = user.ProfileImageUrl,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    private static UserSummaryResponse ToUserSummaryResponse(User user)
    {
        return new UserSummaryResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType.ToString(),
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
