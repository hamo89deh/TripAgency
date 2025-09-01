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

        // التحقق من أن المدة بين StartPackageTripDate و EndPackageTripDate تتطابق مع PackageTrip.Duration (±1 يوم)
        RuleFor(dto => dto)
            .MustAsync(async (dto, cancellation) =>
            {
                var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                    .Where(p => p.Id == dto.PackageTripId)
                    .Select(p => new { p.Duration })
                    .FirstOrDefaultAsync(cancellation);

                if (packageTrip == null)
                {
                    return false; // سيتم التعامل مع هذا في الخدمة
                }

                var tripDuration = (dto.EndPackageTripDate - dto.StartPackageTripDate).Days;
                var expectedDuration = packageTrip.Duration;
                return Math.Abs(tripDuration - expectedDuration) <= 1;
            }).WithMessage(dto =>
            {
                var packageTrip = _packageTripRepositoryAsync.GetTableNoTracking()
                    .Where(p => p.Id == dto.PackageTripId)
                    .Select(p => new { p.Duration })
                    .FirstOrDefaultAsync().Result; // استخدام Result لأن WithMessage ليس async

                return packageTrip != null
                    ? $"The duration between StartPackageTripDate and EndPackageTripDate ({(dto.EndPackageTripDate - dto.StartPackageTripDate).Days} days) must be within ±1 day of the PackageTrip duration ({packageTrip.Duration} days) for PackageTripId: {dto.PackageTripId}."
                    : $"PackageTrip with Id {dto.PackageTripId} not found.";
            });
    }
}