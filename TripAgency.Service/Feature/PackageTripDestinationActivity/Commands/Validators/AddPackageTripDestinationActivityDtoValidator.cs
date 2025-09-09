using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;

namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Commands.Validators
{
    public class AddPackageTripDestinationActivityDtoValidator : AbstractValidator<AddPackageTripDestinationActivityDto>
    {
        public AddPackageTripDestinationActivityDtoValidator()
        {
            RuleFor(dto => dto.PackageTripId)
                .GreaterThan(0).WithMessage("Package trip ID must be a positive integer.");

            RuleFor(dto => dto.DestinationId)
                .GreaterThan(0).WithMessage("Destination ID must be a positive integer.");

            RuleFor(dto => dto.ActivitiesDtos)
                .NotNull().WithMessage("Activities cannot be null.")
                .NotEmpty().WithMessage("At least one activity is required.")
                .Must(activities => activities.Any()).WithMessage("Activities list cannot be empty.");

            RuleForEach(dto => dto.ActivitiesDtos)
                .SetValidator(new PackageTripDestinationActivitiesDtoValidator());
        }
    }

    public class PackageTripDestinationActivitiesDtoValidator : AbstractValidator<PackageTripDestinationActivitiesDto>
    {
        public PackageTripDestinationActivitiesDtoValidator()
        {
            RuleFor(dto => dto.ActivityId)
                .GreaterThan(0).WithMessage("Activity ID must be a positive integer.");

            RuleFor(dto => dto.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    }
}
