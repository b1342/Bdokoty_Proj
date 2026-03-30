using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Users.DTOs.Requests;
using WorkshowcaseApi.Features.Users.DTOs.Responses;

namespace WorkshowcaseApi.Features.Users.Services;

public interface IUserService
{
    Task<UserResponse> UpdateMeAsync(Guid callerId, UpdateUserRequest request);

    Task ChangePasswordAsync(Guid callerId, ChangePasswordRequest request);

    Task<PagedResponse<UserSummaryResponse>> GetAllAsync(UserListQuery query);

    Task<UserResponse> GetByIdAsync(Guid id);

    Task<UserResponse> UpdateStatusAsync(Guid adminId, Guid targetId, UpdateStatusRequest request);

    Task DeleteAsync(Guid adminId, Guid targetId);
}
