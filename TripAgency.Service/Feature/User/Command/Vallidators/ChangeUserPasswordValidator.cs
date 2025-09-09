using FluentValidation;

namespace TripAgency.Service.Feature.User.Command.Vallidators
{
    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPassword>
    {
        public ChangeUserPasswordValidator()
        {
            RuleFor(dto => dto.UserId)
                .GreaterThan(0).WithMessage("User ID must be a positive integer.");

            RuleFor(dto => dto.CurrentPassword)
                .NotNull().WithMessage("Current password cannot be null.")
                .NotEmpty().WithMessage("Current password is required.")
                .Length(8, 50).WithMessage("Current password must be between 8 and 50 characters.");

            RuleFor(dto => dto.NewPassword)
                .NotNull().WithMessage("New password cannot be null.")
                .NotEmpty().WithMessage("New password is required.")
                .Length(8, 50).WithMessage("New password must be between 8 and 50 characters.")
                .Matches("^(?=.*[A-Z])(?=.*\\d)(?=.*[-._@+])[a-zA-Z0-9-._@+]+$")
                .WithMessage("New password must contain at least one uppercase letter, one number, and one special character (-._@+), and only allowed characters (a-z, A-Z, 0-9, -._@+).");

            RuleFor(dto => dto.ConfirmPassword)
                .NotNull().WithMessage("Confirm password cannot be null.")
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(dto => dto.NewPassword).WithMessage("Confirm password must match the new password.");
        }
    }
}
