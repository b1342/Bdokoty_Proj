using FluentValidation;
using WorkshowcaseApi.Features.Works.DTOs.Requests;

namespace WorkshowcaseApi.Features.Works.Validators;

public sealed class UpdateWorkMediaRequestValidator : AbstractValidator<UpdateWorkMediaRequest>
{
    public UpdateWorkMediaRequestValidator()
    {
        RuleFor(x => x.MediaUrl)
            .NotEmpty()
            .MaximumLength(2048)
            .Must(IsValidWorkMediaPath)
            .WithMessage("MediaUrl must be a valid uploaded work media path.")
            .When(x => x.MediaUrl is not null);

        RuleFor(x => x.ExternalMediaId)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.ExternalMediaId));

        RuleFor(x => x.Caption)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Caption));
    }

    private static bool IsValidWorkMediaPath(string? value)
    {
        const string prefix = "/uploads/work-media/";

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        return trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            && trimmed.Length > prefix.Length;
    }
}