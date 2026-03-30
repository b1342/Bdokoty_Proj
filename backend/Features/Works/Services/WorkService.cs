using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Domain.Categories;
using WorkshowcaseApi.Domain.Users;
using WorkshowcaseApi.Domain.Works;
using WorkshowcaseApi.Features.Categories.Repositories;
using WorkshowcaseApi.Features.Tags;
using WorkshowcaseApi.Features.Users.Repositories;
using WorkshowcaseApi.Features.Works.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;
using WorkshowcaseApi.Features.Works.Mappings;
using WorkshowcaseApi.Features.Works.Repositories;

namespace WorkshowcaseApi.Features.Works.Services;

public sealed class WorkService : IWorkService
{
    private readonly IWorkRepository _workRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;

    public WorkService(
        IWorkRepository workRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    }

    public async Task<WorkDetailsResponse> CreateAsync(Guid callerId, CreateWorkRequest request)
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

        await ValidateWorkProfessionalItemsAsync(request.Professionals);
        AddWorkProfessionalsFromRequest(work, request.Professionals, now);
        if (request.TagIds is { Count: > 0 })
        {
            var uniqueTagIds = request.TagIds.Distinct().ToList();
            var tags = await _tagRepository.GetByIdsAsync(uniqueTagIds);
            if (tags.Count != uniqueTagIds.Count)
            {
                throw new ValidationException("One or more TagIds are invalid.");
            }

            foreach (var tag in tags)
            {
                work.WorkTags.Add(new WorkTag { WorkId = work.Id, TagId = tag.Id });
            }
        }

        await _workRepository.AddAsync(work);
        await _workRepository.SaveChangesAsync();

        var created = await _workRepository.GetByIdAsync(work.Id)
            ?? throw new InvalidOperationException("Work was not found after create.");
        return WorkDtoMapper.ToWorkDetailsResponse(created);
    }

    public async Task<WorkDetailsResponse> GetByIdAsync(Guid callerId, Guid workId)
    {
        var work = await _workRepository.GetByIdAsync(workId);
        if (work is null)
        {
            throw new NotFoundException("Work was not found.");
        }

        if (work.Status == WorkStatus.Published)
        {
            return WorkDtoMapper.ToWorkDetailsResponse(work);
        }

        var caller = await _userRepository.GetByIdAsync(callerId);
        var isOwner = caller is not null && caller.Id == work.CreatedByUserId;
        var isAdmin = caller is not null && caller.UserType == UserType.Admin;

        if (!isOwner && !isAdmin)
        {
            throw new NotFoundException("Work was not found.");
        }

        return WorkDtoMapper.ToWorkDetailsResponse(work);
    }

    public async Task<PagedResponse<WorkCardResponse>> GetPublishedFeedAsync(WorkListFilterRequest query)
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
            .Select(WorkDtoMapper.ToWorkCardResponse)
            .ToArray();

        return new PagedResponse<WorkCardResponse>(items, totalCount, page, pageSize);
    }

    public async Task<WorkDetailsResponse> UpdateAsync(Guid callerId, Guid workId, UpdateWorkRequest request)
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

        if (request.PrimaryCategoryId.HasValue && request.PrimaryCategoryId.Value != work.PrimaryCategoryId)
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

        var now = DateTime.UtcNow;
        work.UpdatedAt = now;

        if (request.Professionals is not null)
        {
            await ValidateWorkProfessionalItemsAsync(request.Professionals);
            work.Professionals.Clear();
            AddWorkProfessionalsFromRequest(work, request.Professionals, now);
        }
        if (request.TagIds is not null)
        {
            var uniqueTagIds = request.TagIds.Distinct().ToList();

            if (uniqueTagIds.Count > 0)
            {
                var tags = await _tagRepository.GetByIdsAsync(uniqueTagIds);
                if (tags.Count != uniqueTagIds.Count)
                {
                    throw new ValidationException("One or more TagIds are invalid.");
                }

                work.WorkTags.Clear();
                foreach (var tag in tags)
                {
                work.WorkTags.Add(new WorkTag { WorkId = work.Id, TagId = tag.Id });
            }
        }
        else
        {
            work.WorkTags.Clear();
            }
        }

        await _workRepository.SaveChangesAsync();

        var updated = await _workRepository.GetByIdAsync(workId)
            ?? throw new InvalidOperationException("Work was not found after update.");
        return WorkDtoMapper.ToWorkDetailsResponse(updated);
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

    private async Task ValidateWorkProfessionalItemsAsync(IReadOnlyList<WorkProfessionalItemRequest> items)
    {
        var linkedIds = items
            .Where(x => x.ProfessionalUserId.HasValue)
            .Select(x => x.ProfessionalUserId!.Value)
            .ToList();
        if (linkedIds.Count != linkedIds.Distinct().Count())
        {
            throw new ValidationException("Each professional user may only appear once in the list.");
        }

        foreach (var item in items)
        {
            var externalName = NormalizeNullable(item.ExternalName);

            if (item.ProfessionalUserId.HasValue)
            {
                if (!string.IsNullOrEmpty(externalName))
                {
                    throw new ValidationException(
                        "ExternalName must not be provided when ProfessionalUserId is set.");
                }

                var user = await _userRepository.GetByIdAsync(item.ProfessionalUserId.Value);
                if (user is null)
                {
                    throw new ValidationException("Professional user was not found.");
                }

                if (user.UserType != UserType.Professional)
                {
                    throw new ValidationException("Linked user must be a professional account.");
                }

                if (user.Status != UserStatus.Active)
                {
                    throw new ValidationException("Linked professional user must be active.");
                }
            }
            else if (string.IsNullOrEmpty(externalName))
            {
                throw new ValidationException(
                    "ExternalName is required when ProfessionalUserId is not provided.");
            }
        }
    }

    private static void AddWorkProfessionalsFromRequest(
        Work work,
        IReadOnlyList<WorkProfessionalItemRequest> items,
        DateTime createdAt)
    {
        foreach (var item in items.OrderBy(x => x.SortOrder))
        {
            work.Professionals.Add(new WorkProfessional
            {
                Id = Guid.NewGuid(),
                WorkId = work.Id,
                ProfessionalUserId = item.ProfessionalUserId,
                ExternalName = NormalizeNullable(item.ExternalName),
                ProfessionCategory = NormalizeNullable(item.ProfessionCategory),
                Phone = NormalizeNullable(item.Phone),
                Email = NormalizeNullable(item.Email),
                Whatsapp = NormalizeNullable(item.Whatsapp),
                IsPrimary = item.IsPrimary,
                SortOrder = item.SortOrder,
                CreatedAt = createdAt
            });
        }
    }
}
