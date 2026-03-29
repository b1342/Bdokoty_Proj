using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Works.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;

namespace WorkshowcaseApi.Features.Works.Services;

public interface IWorkService
{
    Task<WorkDetailsResponse> CreateAsync(Guid callerId, CreateWorkRequest request);

    Task<WorkDetailsResponse> GetByIdAsync(Guid callerId, Guid workId);

    Task<PagedResponse<WorkCardResponse>> GetPublishedFeedAsync(WorkListFilterRequest query);

    Task<WorkDetailsResponse> UpdateAsync(Guid callerId, Guid workId, UpdateWorkRequest request);
}
