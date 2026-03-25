using FluentValidation;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Features.Works.DTOs;

namespace WorkshowcaseApi.Features.Works.Validators;

public sealed class WorkListQueryValidator : AbstractValidator<WorkListQuery>
{
    public WorkListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PrimaryCategoryId)
            .Must(value => !value.HasValue || value.Value != System.Guid.Empty)
            .WithMessage("PrimaryCategoryId must not be empty.")
            .When(x => x.PrimaryCategoryId.HasValue);

        RuleFor(x => x.SpaceType)
            .Must(BeValidSpaceType)
            .WithMessage("SpaceType must be a valid value.")
            .When(x => x.SpaceType is not null);

        RuleFor(x => x.CreatedByType)
            .Must(BeValidCreatedByType)
            .WithMessage("CreatedByType must be a valid value.")
            .When(x => x.CreatedByType is not null);
    }

    private static bool BeValidSpaceType(string? value)
    {
        return System.Enum.TryParse<WorkSpaceType>(value?.Trim(), ignoreCase: true, out _);
    }

    private static bool BeValidCreatedByType(string? value)
    {
        return System.Enum.TryParse<WorkCreatedByType>(value?.Trim(), ignoreCase: true, out _);
    }
}
