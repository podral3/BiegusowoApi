using BiegusowoApi.Data.Types;
using FluentValidation;

namespace BiegusowoApi.Features.Listings.Dtos;

public record GetListingsRequest(
    string? Search,
    string? City,
    string? Sort,
    int? SpeciesId,
    int? BreedId,
    int? PriceMin,
    int? PriceMax,
    string? BeforeCursorValue,
    Guid? BeforeListingId,
    int PageSize = 20);

public class GetListingsRequestValidator : AbstractValidator<GetListingsRequest>
{
    public GetListingsRequestValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 30)
            .WithMessage("PageSize must be between 1 and 30.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => x.Search is not null)
            .WithMessage("Search cannot exceed 100 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City is not null)
            .WithMessage("City cannot exceed 100 characters.");

        //RuleFor(x => x.Sort)
        //    .Must(sort => sort is null || AllowedSorts.Contains(
        //        sort,
        //        StringComparer.OrdinalIgnoreCase))
        //    .WithMessage("Sort must be one of: newest, oldest, price_asc, price_desc.");

        RuleFor(x => x.SpeciesId)
            .GreaterThan(0)
            .When(x => x.SpeciesId.HasValue)
            .WithMessage("SpeciesId must be greater than 0.");

        RuleFor(x => x.BreedId)
            .GreaterThan(0)
            .When(x => x.BreedId.HasValue)
            .WithMessage("BreedId must be greater than 0.");

        RuleFor(x => x.PriceMin)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PriceMin.HasValue)
            .WithMessage("PriceMin cannot be negative.");

        RuleFor(x => x.PriceMax)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PriceMax.HasValue)
            .WithMessage("PriceMax cannot be negative.");

        RuleFor(x => x)
            .Must(x =>
                !x.PriceMin.HasValue ||
                !x.PriceMax.HasValue ||
                x.PriceMin <= x.PriceMax)
            .WithMessage("PriceMin cannot be greater than PriceMax.");

        RuleFor(x => x.BeforeCursorValue)
            .NotEmpty()
            .When(x => x.BeforeListingId.HasValue)
            .WithMessage("BeforeCursorValue is required when BeforeListingId is provided.");

        RuleFor(x => x.BeforeListingId)
            .NotEmpty()
            .When(x => x.BeforeCursorValue is not null)
            .WithMessage("BeforeListingId is required when BeforeCursorValue is provided.");
    }
}