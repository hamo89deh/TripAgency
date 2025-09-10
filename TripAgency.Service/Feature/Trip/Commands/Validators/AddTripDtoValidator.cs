using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.Trip.Commands.Validators
{
    public class AddTripDtoValidator : AbstractValidator<AddTripDto>
    {
        public AddTripDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.Description)
                .NotNull().WithMessage("Description cannot be null.")
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(400).WithMessage("Description cannot exceed 400 characters.");

            RuleFor(dto => dto.Image)
                .NotNull().WithMessage("Image is required.")
                .Must(image => image is null ? false: image.Length <= 5 * 1024 * 1024).WithMessage("Image size cannot exceed 5 MB.")
                .Must(image => image is null ? false: new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(image.FileName).ToLower()))
                .WithMessage("Image must be in JPG or PNG format.");
        }
    }
}
