using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.Works.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;

namespace WorkshowcaseApi.Features.Works.Services;

public interface IWorkMediaService
{
    Task<WorkMediaResponse> AddMediaAsync(Guid callerId, Guid workId, AddWorkMediaRequest request);

    Task<WorkMediaResponse> UpdateMediaAsync(Guid callerId, Guid workId, Guid mediaId, UpdateWorkMediaRequest request);

    Task DeleteMediaAsync(Guid callerId, Guid workId, Guid mediaId);
}
