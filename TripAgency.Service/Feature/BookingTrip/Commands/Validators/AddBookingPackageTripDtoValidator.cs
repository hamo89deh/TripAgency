using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.BookingTrip.Commands.Validators
{
    public class AddBookingPackageTripDtoValidator : AbstractValidator<AddBookingPackageTripDto>
    {
        public AddBookingPackageTripDtoValidator()
        {
            RuleFor(dto => dto.PassengerCount)
                .GreaterThan(0).WithMessage("Passenger count must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Passenger count cannot exceed 100.");

            RuleFor(dto => dto.AmountPrice)
                .NotEmpty().WithMessage("AmountPrice is require ")
                .NotNull().WithMessage("AmountPrice cannot be Null.")
                .GreaterThan(0).WithMessage("Amount price must be greater than 0.");

            RuleFor(dto => dto.PackageTripDateId)
                .GreaterThan(0).WithMessage("Package trip date ID must be a positive integer.");

            RuleFor(dto => dto.Notes)
                .MaximumLength(300).WithMessage("Notes cannot exceed 500 characters.");

            RuleFor(dto => dto.PaymentMethodId)
                .GreaterThan(0).WithMessage("Payment method ID must be a positive integer.");
        }
    }  
}
