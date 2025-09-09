using FluentValidation;

namespace TripAgency.Service.Feature.Trip.Commands.Validators
{
    public class AddTripDestinationsDtoValidator : AbstractValidator<AddTripDestinationsDto>
    {
        public AddTripDestinationsDtoValidator()
        {
            RuleFor(dto => dto.TripId)
                .GreaterThan(0).WithMessage("Trip ID must be a positive integer.");

            RuleFor(dto => dto.DestinationIdDto)
                .NotNull().WithMessage("Destinations cannot be null.")
                .NotEmpty().WithMessage("At least one destination is required.")
                .Must(destinations => destinations.Any()).WithMessage("Destinations list cannot be empty.");

            RuleForEach(dto => dto.DestinationIdDto)
                .SetValidator(new AddTripDestinationIdDtoValidator());
        }
    }
}
