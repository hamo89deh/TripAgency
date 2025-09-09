using FluentValidation;

namespace TripAgency.Service.Feature.Trip.Commands.Validators
{
    public class UpdateTripDtoValidator : AbstractValidator<UpdateTripDto>
    {
        public UpdateTripDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.Description)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(400).WithMessage("Description cannot exceed 400 characters.");

            RuleFor(dto => dto.Image)
                .Must(image => image == null || image.Length <= 5 * 1024 * 1024).WithMessage("Image size cannot exceed 5 MB.")
                .Must(image => image == null || new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(image.FileName).ToLower()))
                .WithMessage("Image must be in JPG or PNG format.")
                .When(dto => dto.Image != null);
        }
    }
}
