using System.Threading.Tasks;
using WorkshowcaseApi.Features.Tags.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;

namespace WorkshowcaseApi.Features.Tags.Services;

public interface ITagService
{
    Task<TagResponse> CreateAsync(CreateTagRequest request);
}
