using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Helping;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Feature.DestinationActivity.Queries;
using TripAgency.Service.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TripAgency.Service.Implementations
{
    public class DestinationService : GenericService<Destination, GetDestinationByIdDto, GetDestinationsDto, AddDestinationDto, UpdateDestinationDto>, IDestinationService
    {
        public IMediaService _mediaService { get; set; }

        public DestinationService(IDestinationRepositoryAsync destinationRepository,
                                  ICityRepositoryAsync cityRepository,
                                  IActivityRepositoryAsync activityRepository,
                                  IDestinationActivityRepositoryAsync destinationActivityRepository,                               
                                  IMediaService mediaService,
                                  IMapper mapper) : base(destinationRepository, mapper)
        {
            _destinationRepository = destinationRepository;
            _cityRepository = cityRepository;
            _activityRepository = activityRepository;
            _destinationActivityRepository = destinationActivityRepository;
            _mediaService = mediaService;
            _mapper = mapper;
        }

        public IDestinationRepositoryAsync _destinationRepository { get; }
        public ICityRepositoryAsync _cityRepository { get; }
        public IActivityRepositoryAsync _activityRepository { get; }
        public IDestinationActivityRepositoryAsync _destinationActivityRepository { get; }
        public IMapper _mapper { get; }

        public async Task<Result<IEnumerable<GetDestinationsByCityNameDto>>> GetDestinationsByCityName(string cityName)
        {
            var city = await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c=>c.Name==cityName);
            if(city is null)
            {
                return Result<IEnumerable<GetDestinationsByCityNameDto>>.NotFound($"Not Found City With Name : {cityName}");
            }
            var destinationsByCityName = await _destinationRepository.GetTableNoTracking()
                                                           .Include(d => d.City)
                                                           .Where(d => d.CityId == city.Id).ToListAsync();
            if (destinationsByCityName.Count == 0)
                return Result<IEnumerable<GetDestinationsByCityNameDto>>.NotFound($"Not Found Destinations in {cityName}");

            var destinationsResult = _mapper.Map<List<GetDestinationsByCityNameDto>>(destinationsByCityName);
            return Result<IEnumerable<GetDestinationsByCityNameDto>>.Success(destinationsResult);

        }

        public override async Task<Result<GetDestinationByIdDto>> CreateAsync(AddDestinationDto addDestinationDto)
        {
            var city = await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c => c.Id == addDestinationDto.CityId);
            if (city is null)
                return Result<GetDestinationByIdDto>.NotFound($"Not Found City with Id : {addDestinationDto.CityId}");
                var mapDestination = _mapper.Map<Destination>(addDestinationDto);
            mapDestination.ImageUrl = await _mediaService.UploadMediaAsync("Destination", addDestinationDto.Image);
            await _destinationRepository.AddAsync(mapDestination);

            var resultDestination = _mapper.Map<GetDestinationByIdDto>(mapDestination);

            return Result<GetDestinationByIdDto>.Success(resultDestination);

        }

        public override async Task<Result> UpdateAsync(int id, UpdateDestinationDto updateDestinationDto)
        {
            var destination = await _destinationRepository.GetTableNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (destination is null)
                return Result.NotFound($"Not Found Destination with Id : {id}");

            var city = await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c => c.Id == updateDestinationDto.CityId);
            if (city is null)
                return Result.NotFound($"Not Found City with Id : {updateDestinationDto.CityId}");

            destination.Name = updateDestinationDto.Name;
            destination.CityId = city.Id;
            destination.Description = updateDestinationDto.Description;
            destination.Location = updateDestinationDto.Location;
            var oldImageUrl = "";
            if (updateDestinationDto.Image is not null)
            {
                oldImageUrl=destination.ImageUrl;
                destination.ImageUrl = await _mediaService.UploadMediaAsync("Destination", updateDestinationDto.Image); 
            }
            await _destinationRepository.UpdateAsync(destination);
            if (!string.IsNullOrEmpty(oldImageUrl))
                await _mediaService.DeleteMediaAsync(oldImageUrl);
            return Result.Success();
        }

        public async Task<Result> AddDestinationActivity(int DestinationId, int ActivityId)
        {
            var destination = await _destinationRepository.GetTableNoTracking()
                                                          .FirstOrDefaultAsync(d => d.Id == DestinationId);
            if (destination is null)
                return Result.NotFound($"Not Found Destination with Id : {DestinationId}");

            var activity = await _activityRepository.GetTableNoTracking()
                                                    .FirstOrDefaultAsync(d => d.Id == ActivityId);
            if (activity is null)
                return Result.NotFound($"Not Found Activity with Id : {ActivityId}");

            var destinationActivity = await _destinationActivityRepository.GetTableNoTracking()
                                                                          .FirstOrDefaultAsync(da => da.DestinationId == DestinationId
                                                                                                  && da.ActivityId == ActivityId);
            if (destinationActivity is not null)
            {
                return Result.BadRequest($"Destination With id : {DestinationId} associated with Activity id {ActivityId}");
            }


            await _destinationActivityRepository.AddAsync(new DestinationActivity
            {
                ActivityId = ActivityId,
                DestinationId = DestinationId,
            });
            return Result.Success("Adding Success");

        }
        public async Task<Result> DeleteDestinationActivity(int DestinationId, int ActivityId)
        {
            var destination = await _destinationRepository.GetTableNoTracking()
                                                          .FirstOrDefaultAsync(d => d.Id == DestinationId);
            if (destination is null)
                return Result.NotFound($"Not Found Destination with Id : {DestinationId}");

            var activity = await _activityRepository.GetTableNoTracking()
                                                    .FirstOrDefaultAsync(d => d.Id == ActivityId);
            if (activity is null)
                return Result.NotFound($"Not Found Activity with Id : {ActivityId}");

            var destinationActivity = await _destinationActivityRepository.GetTableNoTracking()
                                                                           .FirstOrDefaultAsync(da => da.DestinationId == DestinationId
                                                                                                   && da.ActivityId == ActivityId);
            if (destinationActivity is null)
            {
                return Result.NotFound($"Not Found Destination With id : {DestinationId} associated with Activity id {ActivityId}");
            }


            await _destinationActivityRepository.DeleteAsync(destinationActivity);
            return Result.Success("Delete Success");

        }

        public async Task<Result<GetDestinationActivitiesByIdDto>> GetDestinationActivitiesByIdDto(int DestinationId)
        {
            var destination = await _destinationRepository.GetTableNoTracking()
                                                          .Where(d => d.Id == DestinationId)
                                                          .Include(d => d.DestinationActivities)
                                                          .ThenInclude(da => da.Activity)
                                                          .FirstOrDefaultAsync();
            if (destination is null)
                return Result<GetDestinationActivitiesByIdDto>.NotFound($"Not Found Destination with Id : {DestinationId}");

            var resultDto = new GetDestinationActivitiesByIdDto()
            {
                DestinationId = DestinationId,
                ActivitiesDto = destination.DestinationActivities.Select(d => new GetActivityByIdDto
                {
                    Id = d.Activity.Id,
                    Description = d.Activity.Description,
                    Name = d.Activity.Name,
                    Price = d.Activity.Price
                })
            };
            return Result<GetDestinationActivitiesByIdDto>.Success(resultDto);

        }

        public async Task<Result<IEnumerable<GetDestinationsDetailsDto>>> GetDestinationsDetails(string? search)
        {
            var query =  _destinationRepository.GetTableNoTracking();
            if (search is not null)
                query = query.ApplySearch(search, new string[] { "Name", "Description" });
            var destinations = await query .Include(x => x.City)
                                     .Include(d => d.DestinationActivities)
                                         .ThenInclude(da => da.Activity)
                                     .ToListAsync();
                                                           
            if (destinations.Count() == 0)
                return Result<IEnumerable < GetDestinationsDetailsDto >>.NotFound("Not Found Any Destinations");

            var resultDto = destinations.Where(x=>x.DestinationActivities.Any()).Select(x => new GetDestinationsDetailsDto()
            {
                Id = x.Id,
                CityId = x.CityId,
                CityName = x.City.Name,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Location = x.Location,
                GetDestinationActivitiesByIds = x.DestinationActivities.Select(d => new GetActivitiesDetailsDto
                {
                    Id = d.Activity.Id,
                    Description = d.Activity.Description,
                    Name = d.Activity.Name,
                })
            });
            return Result<IEnumerable<GetDestinationsDetailsDto>>.Success(resultDto);
        }

        public async Task<Result<PaginatedResult<GetDestinationsDetailsDto>>> GetDestinationsPaginationDetails(string? search, int pageNumber, int pageSize)
        {
            var query = _destinationRepository.GetTableNoTracking();
                                                           
            if (search is not null)
                query = query.ApplySearch(search, new string[] { "Name", "Description" });
            query = query.Include(x => x.City)
                         .Include(d => d.DestinationActivities)
                         .ThenInclude(da => da.Activity)
                         .Where(x => x.DestinationActivities.Any());

            var resultData = await query.Select(x => new GetDestinationsDetailsDto()
            {
                Id = x.Id,
                CityId = x.CityId,
                CityName = x.City.Name,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Location = x.Location,
                GetDestinationActivitiesByIds = x.DestinationActivities.Select(d => new GetActivitiesDetailsDto
                {
                    Id = d.Activity.Id,
                    Description = d.Activity.Description,
                    Name = d.Activity.Name,
                })
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (resultData.TotalCount == 0)
                return Result<PaginatedResult<GetDestinationsDetailsDto>>.NotFound("Not Found Any Destinations");

          
            return Result<PaginatedResult<GetDestinationsDetailsDto>>.Success(resultData);
        }
    }
}
