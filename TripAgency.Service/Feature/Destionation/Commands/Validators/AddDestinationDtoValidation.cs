using FluentValidation;
using TripAgency.Service.Feature.Destination.Commands;
namespace TripAgency.Service.Feature.Destionation.Commands.Validators

{
    public class AddDestinationDtoValidation :AbstractValidator<AddDestinationDto>
    {
        public AddDestinationDtoValidation()
        {
            RuleFor(d => d.Name)
              .NotEmpty().WithMessage("Destination name is required.")
              .NotNull().WithMessage("Destination cannot be null.")
              .Length(2, 50).WithMessage("Destination name must be between 2 and 50 characters.");

            RuleFor(d => d.Description)
                .NotEmpty().WithMessage("Description is required.")
                .NotNull().WithMessage("Description is required.")
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            RuleFor(d => d.Location)
                .NotEmpty().WithMessage("Location is required.")
                .NotNull().WithMessage("Location is required.")
                .MaximumLength(250).WithMessage("Location cannot exceed 250 characters.");
            RuleFor(d => d.CityId)
                  .NotEmpty().WithMessage("City Id is required.")
                  .NotNull().WithMessage("City Id cannot be null.")
                  .GreaterThan(0).WithMessage("City ID must be a positive number.");

        }
    }
}
