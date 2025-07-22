using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.NewFolder1;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Repositories;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripService : GenericService<PackageTrip, GetPackageTripByIdDto, GetPackageTripsDto, AddPackageTripDto, UpdatePackageTripDto>, IPackageTripService
    {
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; set; }
        public ITripRepositoryAsync _tripRepositoryAsync { get; }
        public IMapper _mapper { get; }

        public PackageTripService(IPackageTripRepositoryAsync packagetripRepository, 
                                  ITripRepositoryAsync tripRepository,
                                  IMapper mapper
                                  ) : base(packagetripRepository, mapper)
        {
            _packageTripRepositoryAsync = packagetripRepository;
            _tripRepositoryAsync = tripRepository;
            _mapper = mapper;
        }

        public override async Task<Result<GetPackageTripByIdDto>> CreateAsync(AddPackageTripDto AddDto)
        {
            var trip = await _tripRepositoryAsync.GetByIdAsync(AddDto.TripId);
            if (trip is null)
                return Result<GetPackageTripByIdDto>.NotFound($"Not Found Trip By Id : {AddDto.TripId}");
            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDto UpdateDto)
        {
            var trip = await _tripRepositoryAsync.GetByIdAsync(UpdateDto.TripId);
            if (trip is null)
                return Result.NotFound($"Not Found Trip By Id : {UpdateDto.TripId}");
            return await base.UpdateAsync(id, UpdateDto);
        }

        public async Task<Result<GetPackageTripDestinationsActivitiesDatesDto>> GetPackageTripDestinationsActivitiesDates(int packageTripId , enPackageTripDataStatusDto status)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                             .Where(x => x.Id == packageTripId)
                                                             .Include(x => x.PackageTripDestinations)
                                                             .ThenInclude(x => x.PackageTripDestinationActivities)
                                                             .Include(x => x.PackageTripDates.Where(ptd => ptd.Status ==Global.ConvertEnPackageTripDataStatusDtoToPackageTripDataStatus( status)))
                                                             .FirstOrDefaultAsync();
                                                             
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found PackageTrip With Id : {packageTripId}");
            }



            if (!PackageTrip.PackageTripDestinations.Any())
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found any Destination for PackageTrip : {packageTripId}");
            }

            if (!PackageTrip.PackageTripDestinations.Select(d => d.PackageTripDestinationActivities).Any())
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found any Activities for Destination {string.Join(",", PackageTrip.PackageTripDestinations.Select(d => d.Id))} to PackageTrip : {packageTripId}");
            }

            if (!PackageTrip.PackageTripDates.Any())
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found any Dates for PackageTrip : {packageTripId}");
            }
            var resultDto = new GetPackageTripDestinationsActivitiesDatesDto()
            {
                PackageTripId = packageTripId,
                DestinationsActivitiesDtos = PackageTrip.PackageTripDestinations.Select(d => new PackageTripDestinationsActivitiesDto
                {
                    DayNumber = d.DayNumber,
                    Description = d.Description,
                    Duration = d.Duration,
                    EndTime = d.EndTime,
                    StartTime = d.StartTime,
                    OrderDestination = d.OrderDestination,
                    DestinationId = d.DestinationId,
                    ActivitiesDtos = d.PackageTripDestinationActivities.Select(a => new PackageTripDestinationActivitiesDto
                    {
                        ActivityId = a.ActivityId,
                        Description = a.Description,
                        Duration = a.Duration,
                        EndTime = a.EndTime,
                        OrderActivity = a.OrderActivity,
                        Price = a.Price,
                        StartTime = d.StartTime,
                    })
                }),
                PackageTripDatesDtos = PackageTrip.PackageTripDates.Where(ptd=>ptd.Status== Global.ConvertEnPackageTripDataStatusDtoToPackageTripDataStatus(status)).Select(d => new PackageTripDatesDto
                {
                    Id = d.Id,
                    EndBookingDate = d.EndBookingDate,
                    StartBookingDate = d.StartBookingDate,
                    StartPackageTripDate = d.StartPackageTripDate,
                    EndPackageTripDate = d.EndPackageTripDate ,
                    Status = (enPackageTripDataStatusDto)d.Status

                })
            };
            return Result<GetPackageTripDestinationsActivitiesDatesDto>.Success(resultDto);
        }

    }
}
