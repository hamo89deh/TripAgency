using FluentValidation;
using TripAgency.Service.Feature.City.Command;

namespace TripAgency.Service.Feature.City.Command.Validaters
{
    public class EditCityDtoValidation : AbstractValidator<EditCityDto>
    {
        public EditCityDtoValidation()
        {
            RuleFor(city => city.Name)
                .NotEmpty().WithMessage("City name is required.")
                .Length(2, 50).WithMessage("City name must be between 2 and 50 characters.");
        }
    }
}
