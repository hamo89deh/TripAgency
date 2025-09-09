using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.PackageTrip.Commands.Validators
{
    public class AddPackageTripDtoValidator : AbstractValidator<AddPackageTripDto>
    {
        public AddPackageTripDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.Description)
                .NotNull().WithMessage("Name cannot be null.")
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(400).WithMessage("Description cannot exceed 400 characters.");

            RuleFor(dto => dto.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.");

            RuleFor(dto => dto.Image)
                .NotNull().WithMessage("Image is required.")
                .Must(image => image.Length <= 5 * 1024 * 1024).WithMessage("Image size cannot exceed 5 MB.")
                .Must(image => new[] { ".jpg", ".jpeg", ".png" }.Contains(System.IO.Path.GetExtension(image.FileName).ToLower()))
                .WithMessage("Image must be in JPG or PNG format.");

            RuleFor(dto => dto.MaxCapacity)
                .GreaterThan(0).WithMessage("Max capacity must be greater than 0.")
                .GreaterThanOrEqualTo(dto => dto.MinCapacity).WithMessage("Max capacity must be greater than or equal to min capacity.");

            RuleFor(dto => dto.MinCapacity)
                .GreaterThan(0).WithMessage("Min capacity must be greater than 0.");

            RuleFor(dto => dto.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(dto => dto.TripId)
                .GreaterThan(0).WithMessage("Trip ID must be a positive integer.");
        }
    }
}
