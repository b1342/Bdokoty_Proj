using System;
using System.Threading.Tasks;
using WorkshowcaseApi.Features.Works.DTOs;

namespace WorkshowcaseApi.Features.Works;

public interface IWorkMediaService
{
    Task<WorkMediaResponse> AddMediaAsync(Guid callerId, Guid workId, AddWorkMediaRequest request);

    Task<WorkMediaResponse> UpdateMediaAsync(Guid callerId, Guid workId, Guid mediaId, UpdateWorkMediaRequest request);

    Task DeleteMediaAsync(Guid callerId, Guid workId, Guid mediaId);
}
