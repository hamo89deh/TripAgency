using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Feature.Trip.Commands;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Generic;
using TripAgency.Service.Implemetations;

namespace TripAgency.Service.Implementations
{
    public class TripService : GenericService<Trip, GetTripByIdDto, GetTripsDto, AddTripDto, UpdateTripDto>, ITripService
    {
        private ITripRepositoryAsync _tripRepository { get; set; }
        private IDestinationRepositoryAsync _destinationRepositoryAsync { get; set; }
        public IMediaService _mediaService { get; }
        private ITripDestinationRepositoryAsync _tripDestinationRepositoryAsync { get; set; }
        public IMapper _mapper { get; }

        public TripService(ITripRepositoryAsync tripRepository,
                           IMapper mapper,
                           IDestinationRepositoryAsync destinationRepositoryAsync,
                           IMediaService mediaService,
                           ITripDestinationRepositoryAsync tripDestinationRepositoryAsync) : base(tripRepository, mapper)
        {
            _tripRepository = tripRepository;
            _mapper = mapper;
            _destinationRepositoryAsync = destinationRepositoryAsync;
            _mediaService = mediaService;
            _tripDestinationRepositoryAsync = tripDestinationRepositoryAsync;   
        }
        public async Task<Result<GetTripByIdDto>> GetTripByNameAsync(string name)
        {
            var trip = await _tripRepository.GetTripByName(name);
            if (trip is null)
                return Result<GetTripByIdDto>.NotFound($"Not Found Trip With Name : {name}");
            var tripResult = _mapper.Map<GetTripByIdDto>(trip);
            return Result<GetTripByIdDto>.Success(tripResult);

        }

        public async Task<Result<GetTripDestinationsDto>> AddTripDestinations(AddTripDestinationsDto addTripDestinationsDto)
        {
            var trip = await _tripRepository.GetTableNoTracking()                                           
                                            .Where(t=>t.Id == addTripDestinationsDto.TripId)
                                            .Include(td => td.TripDestinations)
                                            .FirstOrDefaultAsync();
            if (trip is null)
                return Result<GetTripDestinationsDto>.NotFound($"Not Found Trip With Id : {addTripDestinationsDto.TripId}");

            var requestedDestinationIds = addTripDestinationsDto.DestinationIdDto.Select(d => d.DestinationId)
                                                                                 .Distinct()
                                                                                 .ToList();

            if (requestedDestinationIds.Count != addTripDestinationsDto.DestinationIdDto.Count())
            {
                return Result<GetTripDestinationsDto>.BadRequest("Duplicate destination IDs found in the request.");
            }

            var existingDestinations = await _destinationRepositoryAsync.GetTableNoTracking()
                                                                        .Where(d => requestedDestinationIds.Contains(d.Id))
                                                                        .ToListAsync();
          
            if (existingDestinations.Count()!= requestedDestinationIds.Count())
            {      
                var notFoundDestinationIds = requestedDestinationIds.Except(existingDestinations.Select(d => d.Id)).ToList();

                return Result<GetTripDestinationsDto>.NotFound($"One or more destinations not found. Missing Destination Ids: {string.Join(", ", notFoundDestinationIds)}");
            }

            var tripDestinationsToAdd = new List<TripDestination>();
            foreach (var destination in existingDestinations)
            {
                // Check if the trip already has this destination to avoid duplicates
                if (!trip.TripDestinations.Any(td => td.DestinationId == destination.Id))
                {
                    tripDestinationsToAdd.Add(new TripDestination
                    {
                        TripId = trip.Id,
                        DestinationId = destination.Id
                    });
                }
            }

            if (tripDestinationsToAdd.Any())
            {          
                    await _tripDestinationRepositoryAsync.AddRangeAsync(tripDestinationsToAdd);             
            }

            // Map 
            var resultDto = new GetTripDestinationsDto
            {
                TripId = trip.Id,
                DestinationsDto = existingDestinations.Select(d => new GetDestinationByIdDto
                {
                    Id = d.Id,
                    Name = d.Name ,
                    CityId = d.CityId ,
                    Description = d.Description ,   
                    Location = d.Location
                }).ToList()
            };

            return Result<GetTripDestinationsDto>.Success(resultDto);
        }

        public async Task<Result<GetTripDestinationsDto>> GetTripDestinationsById(int TripId)
        {
            var trip = await _tripRepository.GetByIdAsync(TripId);
            if (trip is null)
                return Result<GetTripDestinationsDto>.NotFound($"Not Found Trip With Id : {TripId}");
            
            var tripDestinations = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                        .Where(td => td.TripId == TripId)
                                                                        .Include(d => d.Destination)
                                                                        .ToListAsync();

            var resultDto = new GetTripDestinationsDto
            {
                TripId = trip.Id,                
                DestinationsDto = tripDestinations.Select(d => new GetDestinationByIdDto
                {
                    Id = d.Destination.Id,
                    Description = d.Destination.Description,
                    CityId = d.Destination.CityId,
                    Location = d.Destination.Location,
                    Name = d.Destination.Name,
                    ImageUrl = d.Destination.ImageUrl,
                })

            };
            return Result<GetTripDestinationsDto>.Success(resultDto);
        }

        public async Task<Result> DeleteTripDestination(int tripId, int destinationId)
        {
            var trip = await _tripRepository.GetTableNoTracking().FirstOrDefaultAsync(x=>x.Id == tripId);
            if (trip is null)
                return Result.NotFound($"Not Found Trip With Id : {tripId}");

            var destination = await _destinationRepositoryAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == destinationId);
            if (destination is null)
                return Result.NotFound($"Not Found Destination With Id : {destinationId}");

            var tripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                        .FirstOrDefaultAsync(td => td.TripId == trip.Id && td.DestinationId == destination.Id);
            if(tripDestination is null)
                return Result.NotFound($"Not Found Trip Destination relationship between Trip Id: {tripId} and Destination Id: {destinationId}");

            await _tripDestinationRepositoryAsync.DeleteAsync(tripDestination);
            return Result.Success("Deleted successfully");


        }
        public override async Task<Result<GetTripByIdDto>> CreateAsync(AddTripDto AddDto)
        {
            var mapTrip = _mapper.Map<Trip>(AddDto);
            mapTrip.ImageUrl = await _mediaService.UploadMediaAsync("Trip", AddDto.Image);
            await _tripRepository.AddAsync(mapTrip);

            var resultTrip = _mapper.Map<GetTripByIdDto>(mapTrip);

            return Result<GetTripByIdDto>.Success(resultTrip);
        }
        public override async Task<Result> UpdateAsync(int id, UpdateTripDto UpdateDto)

        {
            var trip = await _tripRepository.GetTableNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (trip is null)
                return Result.NotFound($"Not Found Trip with Id : {id}");

            trip.Name = UpdateDto.Name;
            trip.Description =UpdateDto.Description;
            trip.ImageUrl = await _mediaService.UploadMediaAsync("Trip", UpdateDto.Image);
            await _tripRepository.UpdateAsync(trip);

            return Result.Success("update Successfully");
            
        }
        public override async Task<Result<IEnumerable<GetTripsDto>>> GetAllAsync()
        {
            var trips = await _tripRepository.GetTableNoTracking()
                                             .Include(p => p.PackageTrips)
                                             .ToListAsync();
            var tripResult = new List<GetTripsDto>();
            foreach (var trip in trips)
            {
                tripResult.Add(new GetTripsDto
                {
                    Id = trip.Id,
                    Name = trip.Name,
                    Description = trip.Description,
                    ImageUrl = trip.ImageUrl,
                    CountPackageTrip = trip.PackageTrips.Count()
                });
            }
            return Result<IEnumerable<GetTripsDto>>.Success(tripResult);
        }
    }
}
