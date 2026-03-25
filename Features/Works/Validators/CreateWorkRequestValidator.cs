using System;
using FluentValidation;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Features.Works.DTOs;

namespace WorkshowcaseApi.Features.Works.Validators;

public sealed class CreateWorkRequestValidator : AbstractValidator<CreateWorkRequest>
{
    public CreateWorkRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.PrimaryCategoryId)
            .NotEmpty();

        RuleFor(x => x.SpaceType)
            .NotEmpty()
            .Must(BeValidSpaceType)
            .WithMessage("SpaceType must be a valid value.");

        RuleFor(x => x.CompletionDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("CompletionDate cannot be in the future.")
            .When(x => x.CompletionDate.HasValue);

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(BeValidCreateStatus)
            .WithMessage("Status must be Draft or Published.");
    }

    private static bool BeValidSpaceType(string? value)
    {
        return Enum.TryParse<WorkSpaceType>(value?.Trim(), ignoreCase: true, out _);
    }

    private static bool BeValidCreateStatus(string? value)
    {
        if (!Enum.TryParse<WorkStatus>(value?.Trim(), ignoreCase: true, out var parsed))
        {
            return false;
        }

        return parsed == WorkStatus.Draft || parsed == WorkStatus.Published;
    }
}
