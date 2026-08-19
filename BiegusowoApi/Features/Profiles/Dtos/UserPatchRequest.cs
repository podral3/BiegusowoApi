using FluentValidation;
using System.Text.RegularExpressions;

namespace BiegusowoApi.Features.Users.Dtos;

public class UserPatchRequest
{
    public string? DisplayName { get; set; } = default!;
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; } = default!;
    public int? VoivodeshipId { get; set; }
}

internal class UserPatchRequestValidator : AbstractValidator<UserPatchRequest>
{
    public UserPatchRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Bio)
            .MaximumLength(200);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(BeAValidPolishPhoneNumber)
            .WithMessage("Invalid Polish phone number format.");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);
    }
    private bool BeAValidPolishPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        var cleaned = Regex.Replace(phone, @"[\s-]", "");
        return Regex.IsMatch(cleaned, @"^(\+48|0048)?\d{9}$");
    }
}
