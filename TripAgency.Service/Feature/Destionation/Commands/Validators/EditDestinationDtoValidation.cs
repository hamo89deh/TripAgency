using FluentValidation;
using TripAgency.Service.Feature.Destination.Commands;
namespace TripAgency.Service.Feature.Destionation.Commands.Validators

{
    public class EditDestinationDtoValidation : AbstractValidator<UpdateDestinationDto>
    {
        public EditDestinationDtoValidation()
        {
            RuleFor(d => d.Name)
              .NotEmpty().WithMessage("Destination name is required.")
              .NotNull().WithMessage("Destination name is required.")
              .Length(2, 100).WithMessage("Destination name must be between 2 and 100 characters.");

            RuleFor(d => d.Description)
                .NotEmpty().WithMessage("Description is required.")
                .NotNull().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(d => d.Location)
                .NotEmpty().WithMessage("Location is required.")
                .NotNull().WithMessage("Location is required.")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

            RuleFor(d => d.CityId)
                .GreaterThan(0).WithMessage("City ID must be a positive number.");

        }
    }
}
