using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.Authontication.Valildator
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        public SignInDtoValidator()
        {
            RuleFor(dto => dto.Email)
                .NotNull().WithMessage("Email cannot be null.")
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(dto => dto.Password)
                .NotNull().WithMessage("Password cannot be null.")
                .NotEmpty().WithMessage("Password is required.")
                .Length(8, 50).WithMessage("Password must be between 8 and 50 characters.")
                .Matches("^(?=.*[A-Z])(?=.*\\d)(?=.*[-._@+])[a-zA-Z0-9-._@+]+$")
                .WithMessage("Password must contain at least one uppercase letter, one number, and one special character (-._@+), and only allowed characters (a-z, A-Z, 0-9, -._@+).");
        }
    }
}
