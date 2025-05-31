using FluentValidation;
using TripAgency.Feature.Destination.Commands;
namespace TripAgency.Api.Feature.Destionation.Commands.Validators

{
    public class AddDestinationDtoValidation :AbstractValidator<AddDestinationDto>
    {
        public AddDestinationDtoValidation()
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
    public class EditDestinationDtoValidation : AbstractValidator<EditDestinationDto>
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
