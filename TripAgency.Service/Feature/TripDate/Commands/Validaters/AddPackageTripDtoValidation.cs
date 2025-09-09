using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;

public class AddPackageTripDateDtoValidator : AbstractValidator<AddPackageTripDateDto>
{
    private readonly IPackageTripRepositoryAsync _packageTripRepositoryAsync;

    public AddPackageTripDateDtoValidator(IPackageTripRepositoryAsync packageTripRepositoryAsync)
    {
        _packageTripRepositoryAsync = packageTripRepositoryAsync;

        // التحقق من أن StartPackageTripDate أقل من EndPackageTripDate
        RuleFor(dto => dto.StartPackageTripDate)
            .LessThan(dto => dto.EndPackageTripDate)
            .WithMessage("StartPackageTripDate must be before EndPackageTripDate.");

        // التحقق من أن StartBookingDate أقل من EndBookingDate
        RuleFor(dto => dto.StartBookingDate)
            .LessThan(dto => dto.EndBookingDate)
            .WithMessage("StartBookingDate must be before EndBookingDate.");

        // التحقق من أن StartBookingDate و EndBookingDate قبل StartPackageTripDate و EndPackageTripDate
        RuleFor(dto => dto.StartBookingDate)
            .LessThan(dto => dto.StartPackageTripDate)
            .WithMessage("StartBookingDate must be before StartPackageTripDate.");

        RuleFor(dto => dto.EndBookingDate)
            .LessThan(dto => dto.StartPackageTripDate)
            .WithMessage("EndBookingDate must be before StartPackageTripDate.");
        RuleFor(dto => dto.PackageTripId)
           .GreaterThan(0).WithMessage("Package trip ID must be a positive integer.");

    }
}
