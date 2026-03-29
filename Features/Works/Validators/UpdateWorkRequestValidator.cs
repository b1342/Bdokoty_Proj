using System;
using FluentValidation;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Features.Works.DTOs.Requests;

namespace WorkshowcaseApi.Features.Works.Validators;

public sealed class UpdateWorkRequestValidator : AbstractValidator<UpdateWorkRequest>
{
    public UpdateWorkRequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("Title must not be empty.")
            .MaximumLength(150)
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.PrimaryCategoryId)
            .Must(value => !value.HasValue || value.Value != Guid.Empty)
            .WithMessage("PrimaryCategoryId must not be empty.")
            .When(x => x.PrimaryCategoryId.HasValue);

        RuleFor(x => x.SpaceType)
            .Must(BeValidSpaceType)
            .WithMessage("SpaceType must be a valid value.")
            .When(x => x.SpaceType is not null);

        RuleFor(x => x.CompletionDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("CompletionDate cannot be in the future.")
            .When(x => x.CompletionDate.HasValue);

        RuleFor(x => x.Status)
            .Must(BeValidStatus)
            .WithMessage("Status must be a valid value.")
            .When(x => x.Status is not null);
    }

    private static bool BeValidSpaceType(string? value)
    {
        return Enum.TryParse<WorkSpaceType>(value?.Trim(), ignoreCase: true, out _);
    }

    private static bool BeValidStatus(string? value)
    {
        return Enum.TryParse<WorkStatus>(value?.Trim(), ignoreCase: true, out _);
    }
}
