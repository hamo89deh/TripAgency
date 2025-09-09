using FluentValidation;

namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Commands.Validators
{
    public class UpdatePackageTripDestinationActivityDtoValidator : AbstractValidator<UpdatePackageTripDestinationActivityDto>
    {
        public UpdatePackageTripDestinationActivityDtoValidator()
        {
            RuleFor(dto => dto.PackageTripId)
                .GreaterThan(0).WithMessage("Package trip ID must be a positive integer.");

            RuleFor(dto => dto.DestinationId)
                .GreaterThan(0).WithMessage("Destination ID must be a positive integer.");

            RuleFor(dto => dto.ActivityId)
                 .GreaterThan(0).WithMessage("Activity ID must be a positive integer.");

            RuleFor(dto => dto.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    
    }
}
