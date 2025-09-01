using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Repositories;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripDestinationService : WriteService<PackageTripDestination, AddPackageTripDestinationDto, UpdatePackageTripDestinationDto, GetPackageTripDestinationsDto>, IPackageTripDestinationService
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
        public override async Task<Result<GetPackageTripDestinationsDto>> CreateAsync(AddPackageTripDestinationDto AddDto)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetByIdAsync(AddDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationsDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }


            var PackageTripDate = await _packageTripDateRepositoryAsync.GetTableNoTracking()
                                                               .Where(t => t.PackageTripId == PackageTrip.Id)
                                                               .Take(2)
                                                               .ToListAsync();

            if (PackageTripDate.Any())
            {
                if (PackageTripDate.Count() != 1 || PackageTripDate[0].Status != Data.Enums.PackageTripDateStatus.Draft)
                {
                    return Result<GetPackageTripDestinationsDto>.BadRequest($"You Cann't Add Any New Destination For PackageTrip With Id {PackageTrip.Id} Because you have already Published it Before");
                }
            }

            var DistinctDestinationIds = AddDto.DestinationsDto.Select(a => a.DestinationId)
                                          .Distinct()
                                          .ToList();

            if (DistinctDestinationIds.Count() != AddDto.DestinationsDto.Count())
            {
                return Result<GetPackageTripDestinationsDto>.BadRequest($"Duplicate Destination Ids found in the request. ");
            }

            var existDistinations = await _destinationRepositoryAsync.GetTableNoTracking()
                                                                  .Where(a => DistinctDestinationIds.Contains(a.Id))
                                                                  .ToListAsync();

            if (DistinctDestinationIds.Count() != existDistinations.Count())
            {
                var notFoundDestinationIds = DistinctDestinationIds.Except(existDistinations.Select(d => d.Id));
                return Result<GetPackageTripDestinationsDto>.NotFound($"One or More from Destinations Not found ,Missing Destinatio Ids : {string.Join(',', notFoundDestinationIds)} ");
            }

            var TripDestinations = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                     .Where(da => da.TripId == PackageTrip.TripId
                                                                                && existDistinations.Select(ea => ea.Id).Contains(da.DestinationId))
                                                                     .Include(d => d.Destination)
                                                                     .ToListAsync();

            if (TripDestinations.Count() != existDistinations.Count())
            {
                var notFoundTripDestination = existDistinations.Select(a => a.Id)
                                                                 .Except(TripDestinations.Select(d => d.DestinationId)).ToList();

                return Result<GetPackageTripDestinationsDto>.BadRequest($"The Trip With Id : {PackageTrip.TripId} Not Associated With Destination ids {string.Join(',', notFoundTripDestination)}");
            }


            var existPackageTripDistinations = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                 .Where(a => a.PackageTripId ==PackageTrip.Id)
                                                                 .ToListAsync();
            var PreDestinationAdding = existPackageTripDistinations.Where(x => DistinctDestinationIds.Contains(x.DestinationId));
            if (PreDestinationAdding.Count() != 0)
            {
                return Result<GetPackageTripDestinationsDto>.BadRequest($"the Destination With Id : {string.Join(',', PreDestinationAdding.Select(x => x.DestinationId))} Adding Before");
            }
            foreach (var destination in existDistinations)
            {
                await _packageTripDestinationRepoAsync.AddAsync(new PackageTripDestination
                {
                    DestinationId = destination.Id,
                    PackageTripId = PackageTrip.Id,
                });
            }
            var DestinationDto = new List<PackageTripDestinationDto>();
            foreach (var destination in AddDto.DestinationsDto)
            {
                DestinationDto.Add(new PackageTripDestinationDto
                {
                    DestinationId = destination.DestinationId
                });
            }

            var result = new GetPackageTripDestinationsDto
            {
                PackageTripId = PackageTrip.Id,
                DestinationDto = DestinationDto
            };
            return Result<GetPackageTripDestinationsDto>.Success(result);
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
                                                                .ThenInclude(x=>x.Destination)
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
                        Name= d.Destination.Name,
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
                                                                               .ThenInclude(x=>x.Activity)
                                                                               .FirstOrDefaultAsync();
            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found packageTrip wiht id : {packageTripId} associated with Destination id {destinationId} ");
            }
            GetPackageTripDestinationActivitiesDto restulDto;
            if (!PackageTripDestination.PackageTripDestinationActivities.Any())
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found Any Activities For destination with id :{destinationId} and PackageTrip with id :{packageTripId}");
            }
            restulDto = new GetPackageTripDestinationActivitiesDto()
            {
                DestinationId = PackageTripDestination.DestinationId,
                PackageTripId = PackageTripDestination.PackageTripId,
                ActivitiesDtos = PackageTripDestination.PackageTripDestinationActivities.Select(a => new GetPackageTripDestinationActivityDto
                {
                    //Description = a.Description,
                    //Duration = a.Duration,
                    //EndTime = a.EndTime,
                    //OrderActivity = a.OrderActivity,
                    //StartTime = a.StartTime,
                    Name = a.Activity.Name,
                    ActivityId = a.ActivityId,
                    Price = a.Price,

                })
            };
            return Result<GetPackageTripDestinationActivitiesDto>.Success(restulDto);

        }

        public async Task<Result<GetPackageTripDestinationsActivitiesDto>> GetPackageTripDestinationsActivities(int packageTripId)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                             .Where(x => x.Id == packageTripId)
                                                             .Include(x => x.PackageTripDestinations)
                                                             .ThenInclude(x=>x.PackageTripDestinationActivities)
                                                             .ThenInclude(x=>x.Activity)
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
                            ActivitiesDtos = d.PackageTripDestinationActivities.Select(a => new GetPackageTripDestinationActivityDto
                            {
                                //Description = a.Description,
                                //Duration = a.Duration,
                                //EndTime = a.EndTime,
                                //OrderActivity = a.OrderActivity,
                                //StartTime = d.StartTime,
                                Name = a.Activity.Name,
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
