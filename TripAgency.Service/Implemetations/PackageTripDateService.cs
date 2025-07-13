using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.NetworkInformation;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.NewFolder1;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripDateService : ReadAndAddService<PackageTripDate, GetPackageTripDateByIdDto, GetPackageTripDatesDto, AddPackageTripDateDto>, IPackageTripDateService
    {
        private IPackageTripDateRepositoryAsync _tripDateRepositoryAsync { get; }
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; }  
        private IBookingTripRepositoryAsync _bookingTripRepositoryAsync { get; }
        private INotificationService _notificationServiceAsync { get; }
        private IMapper _mapper { get; }

        public PackageTripDateService(IPackageTripDateRepositoryAsync tripDateRepository,
                              IMapper mapper,
                              IPackageTripRepositoryAsync packageTripRepository,
                              INotificationService notificationServiceAsync,
                              IBookingTripRepositoryAsync bookingTripRepository
                              ) : base(tripDateRepository, mapper)
        {
            _tripDateRepositoryAsync = tripDateRepository;
            _mapper = mapper;
            _packageTripRepositoryAsync = packageTripRepository;
            _notificationServiceAsync = notificationServiceAsync;
            _bookingTripRepositoryAsync = bookingTripRepository;
        }
        public override async Task<Result<GetPackageTripDateByIdDto>> CreateAsync(AddPackageTripDateDto AddDto)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                          .Where(p=>p.Id == AddDto.PackageTripId)
                                                          .Include(p=>p.PackageTripDestinations)
                                                          .ThenInclude(pd=>pd.PackageTripDestinationActivities)
                                                          .FirstOrDefaultAsync();
            if(packageTrip is null)
            {
                return Result<GetPackageTripDateByIdDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
           
            if(!packageTrip.PackageTripDestinations.Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Destinations to this package");
            }

            if(!packageTrip.PackageTripDestinations.Select(pd => pd.PackageTripDestinationActivities).Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Activities to destinations[ {string.Join(',', packageTrip.PackageTripDestinations.Select(d => d.DestinationId))} ]");
            }
           
            var NotFoundPackageTripDestinationActivities = packageTrip.PackageTripDestinations
                                                            .Where(d => d.PackageTripDestinationActivities.Count() == 0);

            if (NotFoundPackageTripDestinationActivities.Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Activities to destinations[ {string.Join(',', NotFoundPackageTripDestinationActivities.Select(d => d.DestinationId))} ]");

            }

            var packageTripDate = new PackageTripDate() 
            {
                AvailableSeats = packageTrip.MaxCapacity,
                StartBookingDate =  AddDto.StartBookingDate,
                EndBookingDate = AddDto.EndBookingDate,
                CreateDate = DateTime.UtcNow,
                IsAvailable = true,
                StartPackageTripDate = AddDto.StartPackageTripDate,
                EndPackageTripDate = AddDto.EndPackageTripDate, 
                PackageTripId = packageTrip.Id,
                Status = PackageTripDataStatus.Draft           
            };

            await _tripDateRepositoryAsync.AddAsync(packageTripDate);

            var returnResult = _mapper.Map<GetPackageTripDateByIdDto>(packageTripDate);

            return Result<GetPackageTripDateByIdDto>.Success(returnResult);
        }

        public async Task<Result> UpdateStatusTripDate(UpdatePackageTripDateDto updateTripDateDto)
        {
            var packageTripDate = await _tripDateRepositoryAsync.GetTableAsTracking()
                                                         .Where(d=>d.Id == updateTripDateDto.Id)
                                                         .Include(tpd=>tpd.PackageTrip)
                                                         .ThenInclude(p=>p.PackageTripDestinations)
                                                         .ThenInclude(p=>p.PackageTripDestinationActivities)
                                                         .FirstOrDefaultAsync();
            if( packageTripDate is null)
                return Result.NotFound($"Not Found Trip Date with Id : {updateTripDateDto.Id}");

           
            if (!CanChangeStatus(packageTripDate.Status, Global.ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(updateTripDateDto.Status)))
            {
                return Result.BadRequest($"Cannot change status from {packageTripDate.Status} to {updateTripDateDto.Status}.");
            }

            if (packageTripDate.Status == PackageTripDataStatus.Draft &&
               updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.Published)
            {
                
                if (packageTripDate.PackageTrip.PackageTripDestinations.Count() == 0)
                {
                    return Result.BadRequest($"Cann't Update Status Package Trip Before Add Destinations And Activities ");
                }

                if (!packageTripDate.PackageTrip.PackageTripDestinations.All(ptd => ptd.PackageTripDestinationActivities.Any()))
                {
                    return Result.BadRequest($"Cann't Update Status Package Trip Before Add Activities For Destinations : {string.Join(',', packageTripDate.PackageTrip.PackageTripDestinations.Where(ptd => ptd.PackageTripDestinationActivities.Count() == 0).Select(d => d.DestinationId))}");
                }
            }

            if (packageTripDate.Status == PackageTripDataStatus.BookingClosed &&
                updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.Published)
            {

                if (packageTripDate.AvailableSeats <= 0)
                {
                    return Result.BadRequest("Cann't republished Because the Package of the Trip is full");
                }
            
                if (packageTripDate.EndBookingDate <= DateTime.UtcNow)
                {
                    return Result.BadRequest("Cann't republished Because the Booking expiring");
                }
            }

            packageTripDate.Status = Global.ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(updateTripDateDto.Status);
            await _tripDateRepositoryAsync.SaveChangesAsync();

            await ExecuteStatusSpecificActions(packageTripDate.Id, packageTripDate.Status);


            return Result.Success();

        }
        public bool CanChangeStatus(PackageTripDataStatus currentStatus, PackageTripDataStatus newStatus)
        {
            switch (currentStatus)
            {
                case PackageTripDataStatus.Draft:
                    return newStatus == PackageTripDataStatus.Published || newStatus == PackageTripDataStatus.Cancelled;
                case PackageTripDataStatus.Published:
                    return newStatus == PackageTripDataStatus.BookingClosed ||
                           newStatus == PackageTripDataStatus.Cancelled ||
                           newStatus == PackageTripDataStatus.Full;
                case PackageTripDataStatus.BookingClosed:
                    return newStatus == PackageTripDataStatus.Ongoing ||
                           newStatus == PackageTripDataStatus.Cancelled;
                case PackageTripDataStatus.Full:
                    return newStatus == PackageTripDataStatus.BookingClosed || newStatus == PackageTripDataStatus.Cancelled;
                case PackageTripDataStatus.Ongoing:
                    return newStatus == PackageTripDataStatus.Completed;
                default:
                    return false; // الحالات الأخرى لا يمكن تغييرها
            }
        }
        public async Task ExecuteStatusSpecificActions(int packageTripDateId, PackageTripDataStatus newStatus)
        {
            switch (newStatus)
            {
                case PackageTripDataStatus.Cancelled:
                    await _notificationServiceAsync.NotifyTripCancellation(packageTripDateId);

                    await ProcessRefundsForCancelledTrip(packageTripDateId);
                    break;

                case PackageTripDataStatus.Completed:
                    await _notificationServiceAsync.NotifyTripCompletion(packageTripDateId);
                    break;
            }
        }
        //TODO
        public async Task ProcessRefundsForCancelledTrip(int packageTripDateId)
        {
            // كود استرداد المبالغ (مثال باستخدام Stripe)
            var bookings = _bookingTripRepositoryAsync.GetBookingTrips(packageTripDateId, PaymentStatus.Paid).ToList();

            foreach (var booking in bookings)
            {
                //var refundResult = await _paymentGateway.ProcessRefundAsync(
                //    booking.PaymentTransactionId,
                //    booking.AmountPaid);

                //booking.PaymentStatus = refundResult.Success ?
                //    PaymentStatus.Refunded :
                //    PaymentStatus.RefundFailed;
            }
        }
    }
}

