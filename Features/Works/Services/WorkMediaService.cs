using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Domain.Works;
using WorkshowcaseApi.Features.Users.Repositories;
using WorkshowcaseApi.Features.Works.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;
using WorkshowcaseApi.Features.Works.Mappings;
using WorkshowcaseApi.Features.Works.Repositories;

namespace WorkshowcaseApi.Features.Works.Services;

public sealed class WorkMediaService : IWorkMediaService
{
    private readonly IWorkMediaRepository _workMediaRepository;
    private readonly IWorkRepository _workRepository;
    private readonly IUserRepository _userRepository;

    public WorkMediaService(
        IWorkMediaRepository workMediaRepository,
        IWorkRepository workRepository,
        IUserRepository userRepository)
    {
        _workMediaRepository = workMediaRepository ?? throw new ArgumentNullException(nameof(workMediaRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<WorkMediaResponse> AddMediaAsync(Guid callerId, Guid workId, AddWorkMediaRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var caller = await GetActiveCallerOrThrowAsync(callerId);
        var work = await _workRepository.GetTrackedByIdAsync(workId);
        if (work is null)
        {
            throw new NotFoundException("Work was not found.");
        }

        EnsureCanManageWork(caller, work);
        work.UpdatedAt = DateTime.UtcNow;

        if (!Enum.TryParse<WorkMediaType>(request.MediaType?.Trim(), ignoreCase: true, out var mediaType))
        {
            throw new ValidationException("Invalid media type value.");
        }

        var media = new WorkMedia
        {
            Id = Guid.NewGuid(),
            WorkId = work.Id,
            Work = work,
            MediaType = mediaType,
            MediaUrl = request.MediaUrl.Trim(),
            ExternalMediaId = NormalizeNullable(request.ExternalMediaId),
            SortOrder = request.SortOrder,
            Caption = NormalizeNullable(request.Caption),
            IsCover = request.IsCover,
            CreatedAt = DateTime.UtcNow
        };

        await _workMediaRepository.AddAsync(media);
        await _workMediaRepository.SaveChangesAsync();

        return WorkDtoMapper.ToWorkMediaResponse(media);
    }

    public async Task<WorkMediaResponse> UpdateMediaAsync(Guid callerId, Guid workId, Guid mediaId, UpdateWorkMediaRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var caller = await GetActiveCallerOrThrowAsync(callerId);
        var media = await _workMediaRepository.GetTrackedByIdAsync(mediaId);
        if (media is null)
        {
            throw new NotFoundException("Work media was not found.");
        }

        if (media.WorkId != workId)
        {
            throw new NotFoundException("Work media was not found.");
        }

        var work = await _workRepository.GetTrackedByIdAsync(media.WorkId);
        if (work is null)
        {
            throw new NotFoundException("Work was not found.");
        }

        EnsureCanManageWork(caller, work);

        if (request.MediaUrl is not null)
        {
            media.MediaUrl = request.MediaUrl.Trim();
        }

        if (request.ExternalMediaId is not null)
        {
            media.ExternalMediaId = NormalizeNullable(request.ExternalMediaId);
        }

        if (request.SortOrder.HasValue)
        {
            media.SortOrder = request.SortOrder.Value;
        }

        if (request.Caption is not null)
        {
            media.Caption = NormalizeNullable(request.Caption);
        }

        if (request.IsCover.HasValue)
        {
            media.IsCover = request.IsCover.Value;
        }

        work.UpdatedAt = DateTime.UtcNow;
        await _workMediaRepository.SaveChangesAsync();

        return WorkDtoMapper.ToWorkMediaResponse(media);
    }

    public async Task DeleteMediaAsync(Guid callerId, Guid workId, Guid mediaId)
    {
        var caller = await GetActiveCallerOrThrowAsync(callerId);
        var media = await _workMediaRepository.GetTrackedByIdAsync(mediaId);
        if (media is null)
        {
            throw new NotFoundException("Work media was not found.");
        }

        if (media.WorkId != workId)
        {
            throw new NotFoundException("Work media was not found.");
        }

        var work = await _workRepository.GetTrackedByIdAsync(media.WorkId);
        if (work is null)
        {
            throw new NotFoundException("Work was not found.");
        }

        EnsureCanManageWork(caller, work);
        work.UpdatedAt = DateTime.UtcNow;
        _workMediaRepository.Remove(media);
        await _workMediaRepository.SaveChangesAsync();
    }

    private async Task<User> GetActiveCallerOrThrowAsync(Guid callerId)
    {
        var caller = await _userRepository.GetByIdAsync(callerId);
        if (caller is null || caller.Status == UserStatus.Deleted)
        {
            throw new NotFoundException("User was not found.");
        }

        if (caller.Status != UserStatus.Active)
        {
            throw new ForbiddenException("Only active users can perform this action.");
        }

        return caller;
    }

    private static void EnsureCanManageWork(User caller, Work work)
    {
        var isOwner = caller.Id == work.CreatedByUserId;
        var isAdmin = caller.UserType == UserType.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenException("You do not have permission to manage media for this work.");
        }
    }

    private static string? NormalizeNullable(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }
}
