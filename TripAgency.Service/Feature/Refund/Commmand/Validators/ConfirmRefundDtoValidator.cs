using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.Refund.Commmand.Validators
{
    public class ConfirmRefundDtoValidator : AbstractValidator<ConfirmRefundDto>
    {
        public ConfirmRefundDtoValidator()
        {
            RuleFor(dto => dto.Id)
                .GreaterThan(0).WithMessage("Refund ID must be a positive integer.");

            RuleFor(dto => dto.TransactionRefunded)
                .NotNull().When(dto => dto.IsConfirm).WithMessage("Transaction reference is required when refund is confirmed.")
                .NotEmpty().When(dto => dto.IsConfirm).WithMessage("Transaction reference cannot be empty when refund is confirmed.")
                .Length(1, 50).When(dto => dto.IsConfirm).WithMessage("Transaction reference must be between 1 and 50 characters when refund is confirmed.");

            RuleFor(dto => dto.AdminNotes)
                .MaximumLength(250).WithMessage("Admin notes cannot exceed 250 characters.");
        }
    }
}
