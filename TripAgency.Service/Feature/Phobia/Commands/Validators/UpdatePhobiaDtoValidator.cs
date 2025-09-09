using FluentValidation;

namespace TripAgency.Service.Feature.Phobia.Commands.Validators
{
    public class UpdatePhobiaDtoValidator : AbstractValidator<UpdatePhobiaDto>
    {
        public UpdatePhobiaDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.Description)
                .NotEmpty().WithMessage("Description cannot be empty.")
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }
}
