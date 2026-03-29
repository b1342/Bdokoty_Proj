using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Models;
using WorkshowcaseApi.Features.Works.DTOs;

namespace WorkshowcaseApi.Features.Works;

public interface IWorksService
{
    Task<WorkResponse> CreateAsync(Guid callerId, CreateWorkRequest request);

    Task<WorkResponse> GetByIdAsync(Guid callerId, Guid workId);

    Task<PagedResponse<WorkListItemResponse>> GetPublishedFeedAsync(WorkListQuery query);

    Task<WorkResponse> UpdateAsync(Guid callerId, Guid workId, UpdateWorkRequest request);
}
