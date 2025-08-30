using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripDestinationService : WriteService<PackageTripDestination, AddPackageTripDestinationDto, UpdatePackageTripDestinationDto, GetPackageTripDestinationByIdDto>, IPackageTripDestinationService
    {
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; set; }
        private IDestinationRepositoryAsync _destinationRepositoryAsync { get; set; }
        private IPackageTripDateRepositoryAsync _packageTripDateRepositoryAsync { get; }
        private ITripDestinationRepositoryAsync _tripDestinationRepositoryAsync { get; set; }
        private IPackageTripDestinationRepositoryAsync _packageTripDestinationRepoAsync { get; set; }
        private IMapper _mapper { get; }

        public PackageTripDestinationService(IPackageTripDestinationRepositoryAsync packageTripDestinationRepoAsync,
                                             IPackageTripRepositoryAsync packageTripRepositoryAsync,
                                             IMapper mapper,
                                             IDestinationRepositoryAsync destinationRepositoryAsync,
                                             IPackageTripDateRepositoryAsync tripDateRepositoryAsync,
                                             ITripDestinationRepositoryAsync tripDestinationRepositoryAsync) : base(packageTripDestinationRepoAsync, mapper)
        {
            _packageTripDestinationRepoAsync = packageTripDestinationRepoAsync;
            _packageTripRepositoryAsync = packageTripRepositoryAsync;
            _mapper = mapper;
            _destinationRepositoryAsync = destinationRepositoryAsync;
            _packageTripDateRepositoryAsync = tripDateRepositoryAsync;
            _tripDestinationRepositoryAsync = tripDestinationRepositoryAsync;
        }
        public override async Task<Result<GetPackageTripDestinationByIdDto>> CreateAsync(AddPackageTripDestinationDto AddDto)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetByIdAsync(AddDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(AddDto.DestinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found Destination With Id : {AddDto.DestinationId}");
            }
            var TripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                       .FirstOrDefaultAsync(x => x.TripId == PackageTrip.TripId  
                                                                                              && x.DestinationId == Destination.Id);
            if (TripDestination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.BadRequest($" Cann't Add Destination With Id : {AddDto.DestinationId} For PackageTrip With Id : {AddDto.PackageTripId}");
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                               .FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id
                                                                                                      && x.DestinationId == Destination.Id);
            if (PackageTripDestination is not null)
            {
                return Result<GetPackageTripDestinationByIdDto>.BadRequest($"Destination Id : {AddDto.DestinationId} is already associated with Package Trip Id: {AddDto.PackageTripId}.");
            }

            var TripDate = await _packageTripDateRepositoryAsync.GetTableNoTracking()
                                                         .FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id);
            if (TripDate is not null)
            {
                return Result<GetPackageTripDestinationByIdDto>.BadRequest($" Cann't Add PackageTripDestination Because you have tripDate with Id : {TripDate.Id} associated with PackageTrip: {PackageTrip.Id}");
            }

            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDestinationDto UpdateDto)
        {

            var PackageTrip = await _packageTripRepositoryAsync.GetByIdAsync(UpdateDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(UpdateDto.DestinationId);
            if (Destination is null)
            {
                return Result.NotFound($"Not Found Destination With Id : {UpdateDto.PackageTripId}");
            }

            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                              .FirstOrDefaultAsync(p => p.PackageTripId == UpdateDto.PackageTripId
                                                                                                   && p.DestinationId == UpdateDto.DestinationId);
            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId} and Destination id  {UpdateDto.DestinationId}");

            }

            var TripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                       .FirstOrDefaultAsync(x => x.TripId == PackageTrip.TripId
                                                                                              && x.DestinationId == Destination.Id);
            if (TripDestination is null)
            {
                return Result.BadRequest($" Cann't Add Destination With Id : {UpdateDto.PackageTripId} For PackageTrip With Id : {PackageTrip.Id}");
            }


            var TripDate = await _packageTripDateRepositoryAsync.GetTableNoTracking()
                                                         .FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id);
            if (TripDate is not null)
            {
                return Result.BadRequest($" Cann't Update PackageTripDestination Because you have tripDate associated with PackageTrip: {PackageTrip.Id}");
            }
            return await base.UpdateAsync(PackageTripDestination.Id, UpdateDto);
        }
        public async Task<Result> DeletePackageTripDestinationAsync(int PackageTripId , int DestinationId)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetByIdAsync(PackageTripId);
            if (packageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With id : {PackageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(DestinationId);
            if (Destination is null)
            {
                return Result.NotFound($"Not Found Destination With Id : {DestinationId}");
            }

            var PackageTripDate = await _packageTripDateRepositoryAsync.GetTableNoTracking()
                                                .Where(t => t.PackageTripId == PackageTripId)
                                                .Take(2)
                                                .ToListAsync();

            if (PackageTripDate.Any())
            {
                if (PackageTripDate.Count() != 1 || PackageTripDate[0].Status != Data.Enums.PackageTripDateStatus.Draft)
                {
                    return Result.BadRequest($"You Cann't  Delete any Destination For PackageTrip With Id : {PackageTripId} Because you have already Published it Before");
                }
            }

            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                               .Where(x => x.PackageTripId == PackageTripId
                                                                                                      && x.DestinationId == DestinationId)
                                                                               .Include(x => x.PackageTripDestinationActivities)
                                                                               .FirstOrDefaultAsync();
            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found packageTrip With id : {PackageTripId} Related with Destination id {DestinationId} ");
            }
       
            
            if (PackageTripDestination.PackageTripDestinationActivities.Count() != 0 )
            {
                return Result.BadRequest($"Cann't Delete Destination with Id : {PackageTripDestination.DestinationId} For PackageTrip With Id : {PackageTripId} Because already have Activities With Ids : {string.Join(',' , PackageTripDestination.PackageTripDestinationActivities.Select(d=>d.ActivityId))} Related before");

            }

            await _packageTripDestinationRepoAsync.DeleteAsync(PackageTripDestination);
            return Result.Success();
        }

        public async Task<Result<GetPackageTripDestinationByIdDto>> GetPackageTripDestination(int packageTripId, int destinationId)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetByIdAsync(packageTripId);
            if (packageTrip is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found PackageTrip With id : {packageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(destinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found Destination With Id : {destinationId}");
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.PackageTripId == packageTripId && x.DestinationId == destinationId);
            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found packageTrip wiht id : {packageTripId} associated with Destination id {destinationId} ");
            }
            var resultDto = _mapper.Map<GetPackageTripDestinationByIdDto>(PackageTripDestination);
            return Result<GetPackageTripDestinationByIdDto>.Success(resultDto);
        }

        public async Task<Result<GetPackageTripDestinationsDto>> GetPackageTripDestinations(int packageTripId)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                               .Where(x=>x.Id==packageTripId)
                                                               .Include(x=>x.PackageTripDestinations)
                                                               .FirstOrDefaultAsync();
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationsDto>.NotFound($"Not Found PackageTrip With Id : {packageTripId}");
            }
            GetPackageTripDestinationsDto resultDto;
            if (PackageTrip.PackageTripDestinations.Any())
            {
                 resultDto = new GetPackageTripDestinationsDto()
                 {
                    PackageTripId = packageTripId,
                    DestinationDto = PackageTrip.PackageTripDestinations.Select(d => new PackageTripDestinationDto
                    {
                        //DayNumber = d.DayNumber,
                        //Duration = d.Duration,
                        //EndTime = d.EndTime,
                        //StartTime = d.StartTime,
                        //OrderDestination = d.OrderDestination,
                        //Description = d.Description,
                        DestinationId = d.DestinationId,
                    })
                 };
            }
            else
            {
                resultDto = new GetPackageTripDestinationsDto()
                {
                    DestinationDto = [],
                    PackageTripId = packageTripId

                };
            }
            
            return Result<GetPackageTripDestinationsDto>.Success(resultDto);
        }

        public async Task<Result<GetPackageTripDestinationActivitiesDto>> GetPackageTripDestinationActivities(int packageTripId, int destinationId)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetByIdAsync(packageTripId);
            if (packageTrip is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found PackageTrip With id : {packageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(destinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found Destination With Id : {destinationId}");
            }

            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                               .Where(x => x.PackageTripId == packageTripId 
                                                                                                      && x.DestinationId == destinationId)
                                                                               .Include(x=>x.PackageTripDestinationActivities)
                                                                               .FirstOrDefaultAsync();
            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found packageTrip wiht id : {packageTripId} associated with Destination id {destinationId} ");
            }
            GetPackageTripDestinationActivitiesDto restulDto;
            if (PackageTripDestination.PackageTripDestinationActivities.Any())
            {
                restulDto = new GetPackageTripDestinationActivitiesDto()
                {
                    DestinationId = PackageTripDestination.DestinationId,
                    PackageTripId = PackageTripDestination.PackageTripId,
                    ActivitiesDtos = PackageTripDestination.PackageTripDestinationActivities.Select(a => new PackageTripDestinationActivitiesDto
                    {
                        //Description = a.Description,
                        //Duration = a.Duration,
                        //EndTime = a.EndTime,
                        //OrderActivity = a.OrderActivity,
                        //StartTime = a.StartTime,
                        ActivityId = a.ActivityId,
                        Price = a.Price,

                    })
                };
            }
            else
            {
                restulDto = new GetPackageTripDestinationActivitiesDto()
                {
                    DestinationId = PackageTripDestination.DestinationId,
                    PackageTripId = PackageTripDestination.PackageTripId,
                    ActivitiesDtos = []

                };
            }
            return Result<GetPackageTripDestinationActivitiesDto>.Success(restulDto);

        }

        public async Task<Result<GetPackageTripDestinationsActivitiesDto>> GetPackageTripDestinationsActivities(int packageTripId)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                             .Where(x => x.Id == packageTripId)
                                                             .Include(x => x.PackageTripDestinations)
                                                             .ThenInclude(x=>x.PackageTripDestinationActivities)
                                                             .FirstOrDefaultAsync();
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationsActivitiesDto>.NotFound($"Not Found PackageTrip With Id : {packageTripId}");
            }
            GetPackageTripDestinationsActivitiesDto resultDto;
            if (PackageTrip.PackageTripDestinations.Any())
            {
                if (PackageTrip.PackageTripDestinations.Select(d => d.PackageTripDestinationActivities).Any())
                {
                    resultDto = new GetPackageTripDestinationsActivitiesDto()
                    {
                        PackageTripId = packageTripId,
                        DestinationsActivitiesDtos = PackageTrip.PackageTripDestinations.Select(d => new PackageTripDestinationsActivitiesDto
                        {
                            //DayNumber = d.DayNumber,
                            //Description = d.Description,
                            //Duration = d.Duration,
                            //EndTime = d.EndTime,
                            //StartTime = d.StartTime,
                            //OrderDestination = d.OrderDestination,
                            DestinationId = d.DestinationId,
                            ActivitiesDtos = d.PackageTripDestinationActivities.Select(a => new PackageTripDestinationActivitiesDto
                            {
                                //Description = a.Description,
                                //Duration = a.Duration,
                                //EndTime = a.EndTime,
                                //OrderActivity = a.OrderActivity,
                                //StartTime = d.StartTime,
                                Price = a.Price,
                                ActivityId = a.ActivityId,
                            })
                        })
                    };
                }
                else
                {
                    resultDto = new GetPackageTripDestinationsActivitiesDto()
                    {
                        PackageTripId = packageTripId,
                        DestinationsActivitiesDtos = PackageTrip.PackageTripDestinations.Select(d => new PackageTripDestinationsActivitiesDto
                        {
                            //DayNumber = d.DayNumber,
                            //Description = d.Description,
                            //Duration = d.Duration,
                            //EndTime = d.EndTime,
                            //StartTime = d.StartTime,
                            //OrderDestination = d.OrderDestination,
                            DestinationId = d.DestinationId,
                            ActivitiesDtos = []
                            
                        })
                    }; 
                }

            }
            else
            {
                resultDto = new GetPackageTripDestinationsActivitiesDto()
                {
                    PackageTripId = packageTripId,
                    DestinationsActivitiesDtos = []

                };
            }

            return Result<GetPackageTripDestinationsActivitiesDto>.Success(resultDto);
        }

     }
}
