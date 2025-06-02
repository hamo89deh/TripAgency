using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class DestinationService :GenericService<Destination,GetDestinationByIdDto , GetDestinationsDto,AddDestinationDto ,UpdateDestinationDto> ,  IDestinationService
    {
        public DestinationService(IDestinationRepositoryAsync destinationRepository ,
                                  ICityRepositoryAsync cityRepository,
                                  IMapper mapper):base(destinationRepository , mapper) 
        {
            _destinationRepository = destinationRepository;
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public IDestinationRepositoryAsync _destinationRepository { get; }
        public ICityRepositoryAsync _cityRepository { get; }
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


            var destinationsResult = _mapper.Map<Destination>(updateDestinationDto);
            await _destinationRepository.UpdateAsync(destinationsResult);
            return Result.Success();
        }
    }
}
