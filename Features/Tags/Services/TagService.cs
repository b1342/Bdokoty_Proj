using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Exceptions;
using WorkshowcaseApi.Domain.Works;
using WorkshowcaseApi.Features.Tags.DTOs.Requests;
using WorkshowcaseApi.Features.Works.DTOs.Responses;

namespace WorkshowcaseApi.Features.Tags.Services;

public sealed class TagService : ITagService
{
    private static readonly Regex MultiWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    }

    public async Task<TagResponse> CreateAsync(CreateTagRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var displayName = NormalizeWhitespace(request.Name);
        var normalizedName = displayName.ToLowerInvariant();

        var existing = await _tagRepository.GetByNormalizedNameAsync(normalizedName);
        if (existing is not null)
        {
            throw new ConflictException("Tag with this name already exists.");
        }

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = displayName,
            NormalizedName = normalizedName,
            CreatedAt = DateTime.UtcNow
        };

        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync();

        return new TagResponse { Id = tag.Id, Name = tag.Name };
    }

    private static string NormalizeWhitespace(string rawValue)
    {
        var value = rawValue ?? string.Empty;
        return MultiWhitespaceRegex.Replace(value.Trim(), " ");
    }
}
