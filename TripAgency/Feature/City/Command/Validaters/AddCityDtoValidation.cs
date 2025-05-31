using FluentValidation;
using TripAgency.Feature.City.Command;

namespace TripAgency.Api.Feature.City.Command.Validaters
{

    public class AddCityDtoValidation : AbstractValidator<AddCityDto>
    {
        public AddCityDtoValidation()
        {
            RuleFor(city => city.Name)
                .NotEmpty().WithMessage("City name is required.") 
                .Length(2, 50).WithMessage("City name must be between 2 and 50 characters."); 
        }
    }
}
