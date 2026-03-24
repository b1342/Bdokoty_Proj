using FluentValidation;
using WorkshowcaseApi.Features.Users.DTOs;

namespace WorkshowcaseApi.Features.Users.Validators;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6);
    }
}
