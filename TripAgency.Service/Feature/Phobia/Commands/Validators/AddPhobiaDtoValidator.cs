using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.Phobia.Commands.Validators
{
    public class AddPhobiaDtoValidator : AbstractValidator<AddPhobiaDto>
    {
        public AddPhobiaDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.Description)
                .NotEmpty().WithMessage("Description cannot be empty")
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }
}
