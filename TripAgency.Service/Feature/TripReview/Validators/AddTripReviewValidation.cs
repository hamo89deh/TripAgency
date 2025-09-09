using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.TripReview.Validators
{
    public class AddTripReviewDtoValidator : AbstractValidator<AddTripReviewDto>
    {
        public AddTripReviewDtoValidator()
        {
            RuleFor(dto => dto.PackageTripDateId)
                .GreaterThan(0)
                .WithMessage("PackageTripDateId must be greater than 0.");

            RuleFor(dto => dto.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(dto => dto.Comment)
                .MaximumLength(300).WithMessage("Comment must not exceed 300 characters.");
        }
    }
}
