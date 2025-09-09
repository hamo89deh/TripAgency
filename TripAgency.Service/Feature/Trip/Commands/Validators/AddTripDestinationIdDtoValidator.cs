using FluentValidation;

namespace TripAgency.Service.Feature.Trip.Commands.Validators
{
    public class AddTripDestinationIdDtoValidator : AbstractValidator<AddTripDestinationIdDto>
    {
        public AddTripDestinationIdDtoValidator()
        {
            RuleFor(dto => dto.DestinationId)
                .GreaterThan(0).WithMessage("Destination ID must be a positive integer.");
        }
    }
}
