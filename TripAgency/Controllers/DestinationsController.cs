using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TripAgency.Bases;
using TripAgency.Data.Entities;
using TripAgency.Feature.Destination.Commands;
using TripAgency.Feature.Destination.Queries;
using TripAgency.Service.Abstracts;

namespace TripAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : ControllerBase
    {
        private object cityResult;

        public DestinationsController(IDestinationService destinationService,
                                      ICityService cityService,
                                      IMapper mapper)
        {
            _destinationService = destinationService;
            _cityService = cityService;
            _mapper = mapper;
        }

        public IDestinationService _destinationService { get; }
        public ICityService _cityService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetDestinationsDto>>> GetDestinations()
        {
            var Destinations = await _destinationService.GetAll().ToListAsync();
            if (Destinations.Count == 0)
                return ApiResult<IEnumerable<GetDestinationsDto>>.NotFound();
            var DestinationssResult = _mapper.Map<List<GetDestinationsDto>>(Destinations);
            return ApiResult<IEnumerable<GetDestinationsDto>>.Ok(DestinationssResult);
        }
        [HttpGet("{id}")]
     
        public async Task<ApiResult<GetDestinationByIdDto>> GetDestinationById(int id)
        {
            var destination = await _destinationService.GetByIdAsync(id);
            if (destination is null)
                return ApiResult<GetDestinationByIdDto>.NotFound();

            var destinationResult = _mapper.Map<GetDestinationByIdDto>(destination);

            return ApiResult<GetDestinationByIdDto>.Ok(destinationResult);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddDestination(AddDestinationDto destination)
        {
            var city = _cityService.GetAll().FirstOrDefault(c => c.Id == destination.CityId);
            if (city is null)
                return ApiResult<string>.NotFound($"Not Found City with id : {destination.CityId}");

            var resultDestination = await _destinationService.AddAsync(_mapper.Map<Destination>(destination));
            return ApiResult<string>.Ok($"DestinationId : {resultDestination.Id}");
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateDestination(EditDestinationDto destination)
        {
            var city = _cityService.GetAll().FirstOrDefault(c => c.Id == destination.CityId);
            if (city is null)
                return ApiResult<string>.NotFound($"Not Found City with id : {destination.CityId}");
            var destinationResult = await _destinationService.GetAll().FirstOrDefaultAsync(x => x.Id == destination.Id);
            if (destinationResult is null)
                return ApiResult<string>.NotFound();
            await _destinationService.UpdateAsync(_mapper.Map<Destination>(destination));
            return ApiResult<string>.Ok("", "Success Updated");

        }
        [HttpDelete("{id}")]
        public async Task<ApiResult<string>> DeleteDestination(int id)
        {
            var destinationResult = await _destinationService.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (destinationResult is null)
                return ApiResult<string>.NotFound();

            await _destinationService.DeleteAsync(destinationResult);
            return ApiResult<string>.Ok("", "Success Deleted");
        }
    }
}
