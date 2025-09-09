using FluentValidation;

namespace TripAgency.Service.Feature.BookingTrip.Commands.Validators
{
    public class UpdateBookingPackageTripDtoValidator : AbstractValidator<UpdateBookingTripDto>
    {
        public UpdateBookingPackageTripDtoValidator()
        {
            RuleFor(dto => dto.PassengerCount)
                .GreaterThan(0).WithMessage("Passenger count must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Passenger count cannot exceed 100.");

            RuleFor(dto => dto.Notes)
                .MaximumLength(300).WithMessage("Notes cannot exceed 500 characters.");

        }
    }
}
