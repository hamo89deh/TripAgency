using FluentValidation;

namespace TripAgency.Service.Feature.Hotel.Commands.Validation
{
    public class UpdateHotelDtoValidator : AbstractValidator<UpdateHotelDto>
    {
        public UpdateHotelDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.Phone)
                .NotNull().WithMessage("Phone cannot be null.")
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^[\+]?[0-9\s\-()]{7,20}$").WithMessage("Invalid phone number format.");

            RuleFor(dto => dto.Email)
                .NotNull().WithMessage("Email cannot be null.")
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(dto => dto.Location)
                .NotNull().WithMessage("Location cannot be null.")
                .NotEmpty().WithMessage("Location is required.")
                .Length(2, 250).WithMessage("Location must be between 2 and 250 characters.");

            RuleFor(dto => dto.Rate)
                .InclusiveBetween(1, 5).WithMessage("Rate must be between 1 and 5.");

            RuleFor(dto => dto.MidPriceForOneNight)
                .GreaterThan(0).WithMessage("Mid price for one night must be greater than 0.");

            RuleFor(dto => dto.CityId)
                .GreaterThan(0).WithMessage("City ID must be a positive integer.");
        }
    }
}
