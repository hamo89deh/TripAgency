using FluentValidation;

namespace TripAgency.Service.Feature.Payment.Validators
{
    public class ManualPaymentConfirmationRequestDtoValidator : AbstractValidator<ManualPaymentConfirmationRequestDto>
    {
        public ManualPaymentConfirmationRequestDtoValidator()
        {
            RuleFor(dto => dto.TransactionRef)
                .NotNull().WithMessage("Transaction reference cannot be null.")
                .NotEmpty().WithMessage("Transaction reference is required.")
                .Length(1, 100).WithMessage("Transaction reference must be between 1 and 50 characters.");

            RuleFor(dto => dto.PaymentMethodId)
                .GreaterThan(0).WithMessage("Payment method ID must be a positive integer.");

            RuleFor(dto => dto.VerifiedAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Verified amount must be Greater Than Or Equal 0.");

            RuleFor(dto => dto.AdminNotes)
                .MaximumLength(250).WithMessage("Admin notes cannot exceed 250 characters.");
        }
    }
}
