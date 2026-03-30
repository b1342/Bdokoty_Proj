using FluentValidation;
using WorkshowcaseApi.Features.ProfessionalProfiles.DTOs.Requests;

namespace WorkshowcaseApi.Features.ProfessionalProfiles.Validators;

public sealed class CreateProfessionalProfileRequestValidator : AbstractValidator<CreateProfessionalProfileRequest>
{
    public CreateProfessionalProfileRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PrimaryCategoryId)
            .NotEmpty();

        RuleFor(x => x.ContactPreference)
            .NotEmpty()
            .Must(IsAllowedContactPreference)
            .WithMessage("ContactPreference must be either email or whatsapp.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.ContactEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

        RuleFor(x => x.WebsiteUrl)
            .Must(IsValidAbsoluteUrl)
            .WithMessage("WebsiteUrl must be a valid absolute URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));

        RuleFor(x => x.LogoUrl)
            .Must(IsValidAbsoluteUrl)
            .WithMessage("LogoUrl must be a valid absolute URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));

        RuleFor(x => x.ContactPhone)
            .MaximumLength(30)
            .When(x => x.ContactPhone is not null);

        RuleFor(x => x.WhatsappNumber)
            .MaximumLength(30)
            .When(x => x.WhatsappNumber is not null);

        RuleFor(x => x.SecondaryCategories)
            .Must(categories => categories is null || categories.Count <= 20)
            .WithMessage("SecondaryCategories cannot contain more than 20 items.");

        RuleForEach(x => x.SecondaryCategories)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ServiceAreas)
            .Must(areas => areas is null || areas.Count <= 20)
            .WithMessage("ServiceAreas cannot contain more than 20 items.");

        RuleForEach(x => x.ServiceAreas)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithMessage("ContactEmail is required when ContactPreference is email.")
            .When(x => IsEmailPreference(x.ContactPreference));

        RuleFor(x => x.WhatsappNumber)
            .NotEmpty()
            .WithMessage("WhatsappNumber is required when ContactPreference is whatsapp.")
            .When(x => IsWhatsappPreference(x.ContactPreference));
    }

    private static bool IsAllowedContactPreference(string? contactPreference)
    {
        if (string.IsNullOrWhiteSpace(contactPreference))
        {
            return false;
        }

        return contactPreference.Equals("email", System.StringComparison.OrdinalIgnoreCase) ||
               contactPreference.Equals("whatsapp", System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidAbsoluteUrl(string? value)
    {
        if (!System.Uri.TryCreate(value, System.UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Scheme == System.Uri.UriSchemeHttp || uri.Scheme == System.Uri.UriSchemeHttps;
    }

    private static bool IsEmailPreference(string? contactPreference)
    {
        return contactPreference is not null &&
               contactPreference.Equals("email", System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsWhatsappPreference(string? contactPreference)
    {
        return contactPreference is not null &&
               contactPreference.Equals("whatsapp", System.StringComparison.OrdinalIgnoreCase);
    }
}
