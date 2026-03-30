using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshowcaseApi.Features.Tags;
using WorkshowcaseApi.Features.Tags.DTOs.Requests;
using WorkshowcaseApi.Features.Tags.Services;
using WorkshowcaseApi.Features.Works.DTOs.Responses;

namespace WorkshowcaseApi.Features.Tags.Controllers;

[ApiController]
[Route("api/tags")]
public sealed class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly ITagService _tagService;

    public TagsController(ITagRepository tagRepository, ITagService tagService)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TagResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TagResponse>>> GetAll()
    {
        var tags = await _tagRepository.GetAllAsync();

        var response = tags
            .Select(t => new TagResponse { Id = t.Id, Name = t.Name })
            .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(TagResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TagResponse>> Create([FromBody] CreateTagRequest request)
    {
        var response = await _tagService.CreateAsync(request);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
