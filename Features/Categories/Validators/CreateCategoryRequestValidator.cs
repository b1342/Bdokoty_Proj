using FluentValidation;
using WorkshowcaseApi.Features.Categories.DTOs;

namespace WorkshowcaseApi.Features.Categories.Validators;

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
