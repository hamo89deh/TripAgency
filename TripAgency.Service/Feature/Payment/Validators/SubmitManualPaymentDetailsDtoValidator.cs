using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.Payment.Validators
{
    public class SubmitManualPaymentDetailsDtoValidator : AbstractValidator<SubmitManualPaymentDetailsDto>
    {
        public SubmitManualPaymentDetailsDtoValidator()
        {
            RuleFor(dto => dto.BookingId)
                .GreaterThan(0).WithMessage("Booking ID must be a positive integer.");

            RuleFor(dto => dto.TransactionReference)
                .NotNull().WithMessage("Transaction reference cannot be null.")
                .NotEmpty().WithMessage("Transaction reference is required.")
                .Length(1, 50).WithMessage("Transaction reference must be between 1 and 50 characters.");

            RuleFor(dto => dto.PaidAmount)
                .GreaterThan(0).WithMessage("Paid amount must be greater than 0.");

            RuleFor(dto => dto.PaymentMethodId)
                .GreaterThan(0).WithMessage("Payment method ID must be a positive integer.");

            RuleFor(dto => dto.PaymentDateTime)
                .NotEmpty().WithMessage("Payment date and time is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Payment date and time cannot be in the future.");

            RuleFor(dto => dto.CustomerNotes)
                .MaximumLength(300).WithMessage("Customer notes cannot exceed 300 characters.");
        }
    }
}
