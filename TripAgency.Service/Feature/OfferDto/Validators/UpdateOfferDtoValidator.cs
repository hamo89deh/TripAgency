using FluentValidation;

namespace TripAgency.Service.Feature.OfferDto.Validators
{
    public class UpdateOfferDtoValidator : AbstractValidator<UpdateOfferDto>
    {
        public UpdateOfferDtoValidator()
        {
            RuleFor(dto => dto.OfferName)
                .NotNull().WithMessage("Offer name cannot be null.")
                .NotEmpty().WithMessage("Offer name is required.")
                .Length(2, 100).WithMessage("Offer name must be between 2 and 100 characters.");

            RuleFor(dto => dto.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(dto => dto.StartDate)
                .NotEmpty().WithMessage("Start date is required.");
                //.Must(date => date >= DateTime.Today)
                //.WithMessage("Start date must be today or in the future.");

            RuleFor(dto => dto.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(dto => dto.StartDate).WithMessage("End date must be after the start date.");
        }
    }
}
