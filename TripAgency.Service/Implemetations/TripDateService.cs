using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class TripDateService : ReadAndAddService<TripDate, GetTripDateByIdDto, GetTripDatesDto, AddTripDateDto>, ITripDateService
    {
        private ITripDateRepositoryAsync _tripDateRepository { get; }
        private IPackageTripRepositoryAsync _packageTripRepository { get; } 
        private IPackageTripDestinationRepositoryAsync _packageTripDestinationRepoAsync { get; } 
        private IPackageTripDestinationActivityRepositoryAsync _packageTripDestinationActivityRepoAsync { get; } 
        private IMapper _mapper { get; }

        public TripDateService(ITripDateRepositoryAsync tripDateRepository,
                              IMapper mapper,
                              IPackageTripRepositoryAsync packageTripRepository,
                              IPackageTripDestinationRepositoryAsync _packageTripDestinationRepoAsync,
                              IPackageTripDestinationActivityRepositoryAsync _packageTripDestinationActivityRepoAsync

                              ) : base(tripDateRepository, mapper)
        {
            _tripDateRepository = tripDateRepository;
            _mapper = mapper;
            _packageTripRepository = packageTripRepository;
        }
        public override async Task<Result<GetTripDateByIdDto>> CreateAsync(AddTripDateDto AddDto)
        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                                                          .Where(p=>p.Id == AddDto.PackageTripId)
                                                          .Include(p=>p.PackageTripDestinations)
                                                          .ThenInclude(pd=>pd.PackageTripDestinationActivities)
                                                          .FirstOrDefaultAsync();
            if(packageTrip is null)
            {
                return Result<GetTripDateByIdDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
           
            if(!packageTrip.PackageTripDestinations.Any())
            {
                return Result<GetTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Destinations to this package");
            }

            if (!packageTrip.PackageTripDestinations.Select(pd => pd.PackageTripDestinationActivities).Any())
            {
                return Result<GetTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Activities to destinations[ {string.Join(',', packageTrip.PackageTripDestinations.Select(d => d.DestinationId))} ]");
            }
            var NotFoundPackageTripDestinationActivities = packageTrip.PackageTripDestinations
                                                            .Where(d => d.PackageTripDestinationActivities.Count() == 0);

            if (NotFoundPackageTripDestinationActivities.Any())
            {
                return Result<GetTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Activities to destinations[ {string.Join(',', NotFoundPackageTripDestinationActivities.Select(d => d.DestinationId))} ]");

            }

            return await base.CreateAsync(AddDto);
        }

        //public async Task<Result> UpdateStatusTripDate(UpdateTripDateDto updateTripDateDto)
        //{
        //   var tripDate= await _tripDateRepository.GetByIdAsync(updateTripDateDto.Id);
        //    if (tripDate is null)
        //        return Result.NotFound($"Not Found Trip Date with Id : {updateTripDateDto.Id}");
        //    tripDate.Status = updateTripDateDto.Status;

        //}
        //private (bool , string) CheckUpdateStatues(TripDataStatus NewStatus , TripDataStatus oldStatus)
        //{
        //    if(oldStatus is TripDataStatus.Completed)
        //    {
        //        switch (NewStatus)
        //        {

        //            case TripDataStatus.Available:
        //                return (true, "");
        //            case TripDataStatus.Cancelled:
        //                return (true, "");
        //            case TripDataStatus.Completed:
        //                return (false, "The New Status Same The Old Status");
        //                default:
        //                return (false, "");
        //        };

        //    }
        //    else if (oldStatus is TripDataStatus.Cancelled)
        //    {
        //        return (false, "Cann't Update From Canscelled Status to any type else");

        //    }
        //    else if (oldStatus is TripDataStatus.Available)
        //    {
        //        switch (NewStatus)
        //        {

        //            case TripDataStatus.Available:
        //                return (true, "");
        //            case TripDataStatus.Cancelled:
        //                return (true, "");
        //            case TripDataStatus.Completed:
        //                return (false, "");
        //            default:
        //                return (false, "");
        //        };
        //    }
        //    else
        //    {
        //        return (false, "");

        //    }

        //}
    }
}
