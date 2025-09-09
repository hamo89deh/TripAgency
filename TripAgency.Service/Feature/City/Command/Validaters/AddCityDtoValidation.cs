using FluentValidation;
using TripAgency.Service.Feature.City.Command;

namespace TripAgency.Service.Feature.City.Command.Validaters
{

    public class AddCityDtoValidation : AbstractValidator<AddCityDto>
    {
        public AddCityDtoValidation()
        {
            RuleFor(city => city.Name)
                .NotEmpty().WithMessage("City name is required.") 
                .NotNull().WithMessage("City  cannot be null.") 
                .Length(2, 50).WithMessage("City name must be between 2 and 50 characters."); 
        }
    }
}
