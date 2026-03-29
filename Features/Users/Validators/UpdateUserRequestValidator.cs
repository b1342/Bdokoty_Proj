using FluentValidation;
using WorkshowcaseApi.Features.Users.DTOs.Requests;

namespace WorkshowcaseApi.Features.Users.Validators;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    private const string PhonePattern = @"^[0-9+\-()\s]+$";

    public UpdateUserRequestValidator()
    {
        RuleFor(x => x)
            .Must(request => !string.IsNullOrWhiteSpace(request.FullName) || !string.IsNullOrWhiteSpace(request.Phone))
            .WithMessage("At least one of FullName or Phone must be provided.");

        RuleFor(x => x.FullName)
            .Must(fullName => string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length is >= 2 and <= 100)
            .WithMessage("FullName must be between 2 and 100 characters.");

        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .Must(phone => string.IsNullOrWhiteSpace(phone) || phone.Trim().Length >= 6)
            .WithMessage("Phone must be between 6 and 30 characters.")
            .Must(phone => string.IsNullOrWhiteSpace(phone) || phone.Trim().Length <= 30)
            .WithMessage("Phone must be between 6 and 30 characters.")
            .Must(phone => string.IsNullOrWhiteSpace(phone) || System.Text.RegularExpressions.Regex.IsMatch(phone, PhonePattern))
            .WithMessage("Phone can contain only digits, +, -, (, ), and spaces.");
    }
}
