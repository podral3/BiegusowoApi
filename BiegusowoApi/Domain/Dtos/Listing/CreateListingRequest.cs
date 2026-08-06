using FluentValidation;

namespace BiegusowoApi.Domain.Dtos.Listing;

public record CreateListingRequest(
    string Title,
    string Description,
    double Price,
    bool PriceNegotiable,
    int SpeciesId,
    int BreedId,
    string? BreedNote,
    int VoivodeshipId,
    int ListingTypeId,
    int ListingStatusId,
    string CityName,
    double Longitude,
    double Latitude,
    Dictionary<string,string> ImageOrderIdPair
);

internal class CreateListingRequestValidator : AbstractValidator<CreateListingRequest>
{
    public CreateListingRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(3, 100);
        RuleFor(x => x.Description)
            .Length(10, 5000);
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.SpeciesId)
            .GreaterThan(0);
        RuleFor(x => x.BreedId)
            .GreaterThan(0);
        RuleFor(x => x.VoivodeshipId)
            .GreaterThan(0)
            .LessThan(32);
        RuleFor(x => x.BreedNote)
           .MaximumLength(300)
           .When(x => x.BreedNote is not null);
        RuleFor(x => x.ListingTypeId)
            .GreaterThan(0);
        RuleFor(x => x.ListingStatusId)
            .GreaterThan(0);
        RuleFor(x => x.CityName)
            .NotEmpty()
            .Length(2, 100)
            .Matches(@"^[\p{L}\s\-'.]+$")
            .WithMessage("City name contains invalid characters.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(49.0, 54.9)
            .WithMessage("Latitude must be within Poland's geographic range.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(14.1, 24.2)
            .WithMessage("Longitude must be within Poland's geographic range.");
    }
}
