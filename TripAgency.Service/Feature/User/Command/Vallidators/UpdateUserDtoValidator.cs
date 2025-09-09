using FluentValidation;

namespace TripAgency.Service.Feature.User.Command.Vallidators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(dto => dto.UserName)
                .NotNull().WithMessage("Username cannot be null.")
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Username can only contain letters and numbers.");

            RuleFor(dto => dto.FirstName)
                .NotNull().WithMessage("First name cannot be null.")
                .NotEmpty().WithMessage("First name is required.")
                .Length(2, 100).WithMessage("First name must be between 2 and 100 characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("First name can only contain letters.");

            RuleFor(dto => dto.LastName)
                .NotNull().WithMessage("Last name cannot be null.")
                .NotEmpty().WithMessage("Last name is required.")
                .Length(2, 100).WithMessage("Last name must be between 2 and 100 characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("Last name can only contain letters.");

            RuleFor(dto => dto.Email)
                .NotNull().WithMessage("Email cannot be null.")
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty().When(dto => dto.PhoneNumber != null).WithMessage("Phone number cannot be empty if provided.")
                .Length(7, 15).When(dto => dto.PhoneNumber != null).WithMessage("Phone number must be between 7 and 15 characters.")
                .Matches(@"^\+?[0-9\-]+$").When(dto => dto.PhoneNumber != null).WithMessage("Phone number can only contain numbers, '+', or '-'.");

            RuleFor(dto => dto.Address)
                .NotEmpty().When(dto => dto.Address != null).WithMessage("Address cannot be empty if provided.")
                .MaximumLength(200).When(dto => dto.Address != null).WithMessage("Address cannot exceed 200 characters.");

            RuleFor(dto => dto.Country)
                .NotEmpty().When(dto => dto.Country != null).WithMessage("Country cannot be empty if provided.")
                .MaximumLength(100).When(dto => dto.Country != null).WithMessage("Country cannot exceed 100 characters.");

            RuleFor(dto => dto.Image)
                .Must(image => image == null || image.Length <= 5 * 1024 * 1024).WithMessage("Image size cannot exceed 5 MB.")
                .Must(image => image == null || new[] { ".jpg", ".jpeg", ".png" }.Contains(System.IO.Path.GetExtension(image.FileName).ToLower()))
                .WithMessage("Image must be in JPG or PNG format.")
                .When(dto => dto.Image != null);
        }
    }
}
