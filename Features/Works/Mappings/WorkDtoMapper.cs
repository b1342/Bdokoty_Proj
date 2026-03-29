using System.Linq;
using WorkshowcaseApi.Domain.Works;
using WorkshowcaseApi.Features.Works.DTOs.Responses;

namespace WorkshowcaseApi.Features.Works.Mappings;

public static class WorkDtoMapper
{
    public static WorkDetailsResponse ToWorkDetailsResponse(Work work)
    {
        return new WorkDetailsResponse
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
            Professionals = work.Professionals
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .Select(ToWorkProfessionalResponse)
                .ToList(),
            Status = work.Status.ToString(),
            CreatedAt = work.CreatedAt,
            UpdatedAt = work.UpdatedAt,
            PublishedAt = work.PublishedAt
        };
    }

    public static WorkCardResponse ToWorkCardResponse(Work work)
    {
        return new WorkCardResponse
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

    public static WorkMediaResponse ToWorkMediaResponse(WorkMedia media)
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

    public static WorkProfessionalResponse ToWorkProfessionalResponse(WorkProfessional professional)
    {
        var displayName = professional.ProfessionalUser is not null
            ? professional.ProfessionalUser.FullName
            : professional.ExternalName;

        return new WorkProfessionalResponse
        {
            ProfessionalUserId = professional.ProfessionalUserId,
            DisplayName = displayName,
            ProfessionCategory = professional.ProfessionCategory,
            Phone = professional.Phone,
            Email = professional.Email,
            Whatsapp = professional.Whatsapp,
            IsPrimary = professional.IsPrimary,
            SortOrder = professional.SortOrder
        };
    }
}
