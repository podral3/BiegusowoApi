using FluentValidation;

namespace BiegusowoApi.Features.Accounts.Dtos;

public sealed record SetupAccountRequest(
    string DisplayName,
    string? Bio,
    string? PhoneNumber,
    string? CityName,
    int VoivodeshipId);

public class SetupAccountRequestValidator : AbstractValidator<SetupAccountRequest>
{
    private static readonly HashSet<int> ValidVoivodeshipIds = new()
    {
        2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32
    };

    public SetupAccountRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

        RuleFor(x => x.CityName)
            .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");

        RuleFor(x => x.VoivodeshipId)
            .NotEmpty().WithMessage("Voivodeship is required.")
            .Must(BeValidVoivodeshipId).WithMessage("Invalid voivodeship ID.");
    }

    private static bool BeValidVoivodeshipId(int voivodeshipId)
        => ValidVoivodeshipIds.Contains(voivodeshipId);
}
