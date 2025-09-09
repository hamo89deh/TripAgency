using FluentValidation;
using TripAgency.Service.Feature.TripDate.Commands;

public class UpdatePackageTripDateDtoValidator : AbstractValidator<UpdatePackageTripDateDto>
{
    public UpdatePackageTripDateDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0).WithMessage("Package trip date ID must be a positive integer.");

        RuleFor(dto => dto.Status)
            .IsInEnum().WithMessage("Status must be a valid value from enUpdatePackageTripDataStatusDto.");
    }
}