using FluentValidation;
using WorkshowcaseApi.Features.Users.DTOs;

namespace WorkshowcaseApi.Features.Users.Validators;

public sealed class UpdateStatusRequestValidator : AbstractValidator<UpdateStatusRequest>
{
    public UpdateStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(IsAllowedStatus)
            .WithMessage("Status must be one of: Active, Pending, Blocked.");
    }

    private static bool IsAllowedStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return status.Equals("Active", System.StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Pending", System.StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Blocked", System.StringComparison.OrdinalIgnoreCase);
    }
}
