using FluentValidation;

namespace TripAgency.Service.Feature.Activity.Commands.Validators
{
    public class UpdateActivityDtoValidator : AbstractValidator<UpdateActivityDto>
    {
        public UpdateActivityDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

            RuleFor(dto => dto.Description)
                .NotEmpty().WithMessage("Description is required.")
                .NotNull().WithMessage("Description cannot be Null")
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");


            RuleFor(dto => dto.Price)
                .NotNull().WithMessage("Price cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    }
}
