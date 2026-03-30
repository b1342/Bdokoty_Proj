using FluentValidation;
using WorkshowcaseApi.Features.Auth.DTOs.Requests;

namespace WorkshowcaseApi.Features.Auth.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private const string PhonePattern = @"^[0-9+\-()\s]+$";

    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(128);

        RuleFor(x => x.UserType)
            .NotEmpty()
            .Must(IsAllowedUserType)
            .WithMessage("UserType must be Professional or Client.");

        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .Must(phone => string.IsNullOrWhiteSpace(phone) || phone.Trim().Length >= 6)
            .WithMessage("Phone must be between 6 and 30 characters.")
            .Must(phone => string.IsNullOrWhiteSpace(phone) || phone.Trim().Length <= 30)
            .WithMessage("Phone must be between 6 and 30 characters.")
            .Must(phone => string.IsNullOrWhiteSpace(phone) || System.Text.RegularExpressions.Regex.IsMatch(phone, PhonePattern))
            .WithMessage("Phone can contain only digits, +, -, (, ), and spaces.");
    }

    private static bool IsAllowedUserType(string? userType)
    {
        if (string.IsNullOrWhiteSpace(userType))
        {
            return false;
        }

        return userType.Equals("Professional", System.StringComparison.OrdinalIgnoreCase) ||
               userType.Equals("Client", System.StringComparison.OrdinalIgnoreCase);
    }
}
