using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Users.DTOs;

namespace WorkshowcaseApi.Features.Users;

public interface IUsersService
{
    Task<UserResponse> UpdateMeAsync(Guid callerId, UpdateMeRequest request);

    Task ChangePasswordAsync(Guid callerId, ChangePasswordRequest request);

    Task<PagedResponse<UserSummaryResponse>> GetAllAsync(UserListQuery query);

    Task<UserResponse> GetByIdAsync(Guid id);

    Task<UserResponse> UpdateStatusAsync(Guid adminId, Guid targetId, UpdateStatusRequest request);

    Task DeleteAsync(Guid adminId, Guid targetId);
}
