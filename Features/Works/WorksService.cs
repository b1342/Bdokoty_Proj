using System;
using System.Linq;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Domain.Categories;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Domain.Works;
using WorkshowcaseApi.Features.Categories;
using WorkshowcaseApi.Features.Users;
using WorkshowcaseApi.Features.Works.DTOs;

namespace WorkshowcaseApi.Features.Works;

public sealed class WorksService : IWorksService
{
    private readonly IWorkRepository _workRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public WorksService(
        IWorkRepository workRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<WorkResponse> CreateAsync(Guid callerId, CreateWorkRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var caller = await GetActiveCallerOrThrowAsync(callerId);
        var category = await GetActiveCategoryOrThrowAsync(request.PrimaryCategoryId);

        var status = ParseWorkStatus(request.Status);
        var spaceType = ParseWorkSpaceType(request.SpaceType);
        var createdByType = DeriveCreatedByType(caller);
        var now = DateTime.UtcNow;

        var work = new Work
        {
            Id = Guid.NewGuid(),
            CreatedByUserId = caller.Id,
            PrimaryCategoryId = category.Id,
            Title = request.Title.Trim(),
            Description = NormalizeNullable(request.Description),
            CompletionDate = request.CompletionDate,
            SpaceType = spaceType,
            CreatedByType = createdByType,
            Status = status,
            HasBeforeAfter = request.HasBeforeAfter,
            IsAnonymous = request.IsAnonymous,
            CreatedAt = now,
            UpdatedAt = now,
            PublishedAt = status == WorkStatus.Published ? now : null
        };

        await _workRepository.AddAsync(work);
        await _workRepository.SaveChangesAsync();

        work.PrimaryCategory = category;
        return ToWorkResponse(work);
    }

    public async Task<WorkResponse> GetByIdAsync(Guid callerId, Guid workId)
    {
        var work = await _workRepository.GetByIdAsync(workId);
        if (work is null)
        {
            throw new NotFoundException("Work was not found.");
        }

        if (work.Status == WorkStatus.Published)
        {
            return ToWorkResponse(work);
        }

        var caller = await _userRepository.GetByIdAsync(callerId);
        var isOwner = caller is not null && caller.Id == work.CreatedByUserId;
        var isAdmin = caller is not null && caller.UserType == UserType.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new NotFoundException("Work was not found.");
        }

        return ToWorkResponse(work);
    }

    public async Task<PagedResponse<WorkListItemResponse>> GetPublishedFeedAsync(WorkListQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Page < 1)
        {
            throw new ValidationException("Page must be greater than or equal to 1.");
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            throw new ValidationException("PageSize must be between 1 and 100.");
        }

        var page = query.Page;
        var pageSize = query.PageSize;

        var parsedSpaceType = ParseOptionalEnum<WorkSpaceType>(query.SpaceType);
        var parsedCreatedByType = ParseOptionalEnum<WorkCreatedByType>(query.CreatedByType);

        var works = await _workRepository.GetPublishedAsync(
            query.PrimaryCategoryId,
            parsedSpaceType,
            parsedCreatedByType,
            page,
            pageSize);

        var totalCount = await _workRepository.CountPublishedAsync(
            query.PrimaryCategoryId,
            parsedSpaceType,
            parsedCreatedByType);

        var items = works
            .Select(ToWorkListItemResponse)
            .ToArray();

        return new PagedResponse<WorkListItemResponse>(items, totalCount, page, pageSize);
    }

    public async Task<WorkResponse> UpdateAsync(Guid callerId, Guid workId, UpdateWorkRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var caller = await GetActiveCallerOrThrowAsync(callerId);

        var work = await _workRepository.GetTrackedByIdAsync(workId);
        if (work is null)
        {
            throw new NotFoundException("Work was not found.");
        }

        var isOwner = caller.Id == work.CreatedByUserId;
        var isAdmin = caller.UserType == UserType.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenException("You do not have permission to update this work.");
        }

        if (request.Title is not null)
        {
            work.Title = request.Title.Trim();
        }

        if (request.Description is not null)
        {
            work.Description = NormalizeNullable(request.Description);
        }

        if (request.PrimaryCategoryId.HasValue)
        {
            var category = await GetActiveCategoryOrThrowAsync(request.PrimaryCategoryId.Value);
            work.PrimaryCategoryId = category.Id;
            work.PrimaryCategory = category;
        }

        if (request.SpaceType is not null)
        {
            work.SpaceType = ParseWorkSpaceType(request.SpaceType);
        }

        if (request.CompletionDate.HasValue)
        {
            work.CompletionDate = request.CompletionDate.Value;
        }

        if (request.HasBeforeAfter.HasValue)
        {
            work.HasBeforeAfter = request.HasBeforeAfter.Value;
        }

        if (request.IsAnonymous.HasValue)
        {
            work.IsAnonymous = request.IsAnonymous.Value;
        }

        if (request.Status is not null)
        {
            var newStatus = ParseWorkStatus(request.Status);
            ValidateStatusTransition(work.Status, newStatus);

            if (newStatus == WorkStatus.Published && work.PublishedAt is null)
            {
                work.PublishedAt = DateTime.UtcNow;
            }

            work.Status = newStatus;
        }

        work.UpdatedAt = DateTime.UtcNow;

        await _workRepository.SaveChangesAsync();

        return ToWorkResponse(work);
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

    private async Task<Category> GetActiveCategoryOrThrowAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null || !category.IsActive)
        {
            throw new ValidationException("PrimaryCategoryId is invalid.");
        }

        return category;
    }

    private static WorkCreatedByType DeriveCreatedByType(User caller)
    {
        return caller.UserType switch
        {
            UserType.Professional => WorkCreatedByType.Professional,
            UserType.Client => WorkCreatedByType.Client,
            UserType.Admin => WorkCreatedByType.Admin,
            _ => throw new ValidationException("User type is not supported for creating works.")
        };
    }

    private static void ValidateStatusTransition(WorkStatus current, WorkStatus next)
    {
        if (current == next)
        {
            return;
        }

        var allowed = current switch
        {
            WorkStatus.Draft => next == WorkStatus.Published,
            WorkStatus.Published => next == WorkStatus.Archived,
            WorkStatus.Archived => next == WorkStatus.Published,
            _ => false
        };

        if (!allowed)
        {
            throw new ValidationException($"Cannot transition from {current} to {next}.");
        }
    }

    private static WorkStatus ParseWorkStatus(string? raw)
    {
        if (!Enum.TryParse<WorkStatus>(raw?.Trim(), ignoreCase: true, out var parsed))
        {
            throw new ValidationException("Invalid status value.");
        }

        return parsed;
    }

    private static WorkSpaceType ParseWorkSpaceType(string? raw)
    {
        if (!Enum.TryParse<WorkSpaceType>(raw?.Trim(), ignoreCase: true, out var parsed))
        {
            throw new ValidationException("Invalid space type value.");
        }

        return parsed;
    }

    private static T? ParseOptionalEnum<T>(string? raw) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (Enum.TryParse<T>(raw.Trim(), ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new ValidationException($"Invalid {typeof(T).Name} value.");
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

    private static WorkResponse ToWorkResponse(Work work)
    {
        return new WorkResponse
        {
            Id = work.Id,
            CreatedByUserId = work.CreatedByUserId,
            CreatedByType = work.CreatedByType.ToString(),
            Title = work.Title,
            Description = work.Description,
            PrimaryCategoryId = work.PrimaryCategoryId,
            PrimaryCategoryName = work.PrimaryCategory?.Name ?? string.Empty,
            SpaceType = work.SpaceType.ToString(),
            CompletionDate = work.CompletionDate,
            HasBeforeAfter = work.HasBeforeAfter,
            IsAnonymous = work.IsAnonymous,
            Media = work.Media
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .Select(ToWorkMediaResponse)
                .ToList(),
            Status = work.Status.ToString(),
            CreatedAt = work.CreatedAt,
            UpdatedAt = work.UpdatedAt,
            PublishedAt = work.PublishedAt
        };
    }

    private static WorkMediaResponse ToWorkMediaResponse(WorkMedia media)
    {
        return new WorkMediaResponse
        {
            Id = media.Id,
            WorkId = media.WorkId,
            MediaType = media.MediaType.ToString(),
            MediaUrl = media.MediaUrl,
            ExternalMediaId = media.ExternalMediaId,
            SortOrder = media.SortOrder,
            Caption = media.Caption,
            IsCover = media.IsCover,
            CreatedAt = media.CreatedAt
        };
    }

    private static WorkListItemResponse ToWorkListItemResponse(Work work)
    {
        return new WorkListItemResponse
        {
            Id = work.Id,
            Title = work.Title,
            PrimaryCategoryId = work.PrimaryCategoryId,
            PrimaryCategoryName = work.PrimaryCategory?.Name ?? string.Empty,
            SpaceType = work.SpaceType.ToString(),
            CompletionDate = work.CompletionDate,
            HasBeforeAfter = work.HasBeforeAfter,
            IsAnonymous = work.IsAnonymous,
            CreatedByType = work.CreatedByType.ToString(),
            PublishedAt = work.PublishedAt
        };
    }
}
