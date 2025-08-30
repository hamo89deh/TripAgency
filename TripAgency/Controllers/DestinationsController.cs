using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Feature.DestinationActivity.Queries;

namespace TripAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : ControllerBase
    {
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
            var result = await _destinationService.GetAllAsync();
            if (!result.IsSuccess)
            {
                return this.ToApiResult(result);
            }
            return ApiResult<IEnumerable<GetDestinationsDto>>.Ok(result.Value!);
        }
        [HttpGet("{id}")]

        public async Task<ApiResult<GetDestinationByIdDto>> GetDestinationById(int id)
        {
            var result = await _destinationService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                return this.ToApiResult(result);
            }
            return ApiResult<GetDestinationByIdDto>.Ok(result.Value!);
        }
        [HttpGet("City/{cityName}")]

        public async Task<ApiResult<IEnumerable<GetDestinationsByCityNameDto>>> GetDestinationByCityName(string cityName)
        {
            var result = await _destinationService.GetDestinationsByCityName(cityName);
            if (!result.IsSuccess)
            {
                return this.ToApiResult(result);
            }
            return ApiResult<IEnumerable<GetDestinationsByCityNameDto>>.Ok(result.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<GetDestinationByIdDto>> AddDestination([FromForm]AddDestinationDto destinationDto)
        {
            var result = await _destinationService.CreateAsync(destinationDto);
            if (!result.IsSuccess)
            {
                return this.ToApiResult<GetDestinationByIdDto>(result);
            }
            return ApiResult<GetDestinationByIdDto>.Created(result.Value!);
        }
        [HttpPut("{Id}")]
        public async Task<ApiResult<string>> UpdateDestination(int Id ,[FromForm]UpdateDestinationDto destinationDto)
        {
            var result = await _destinationService.UpdateAsync(Id,destinationDto);
            if (!result.IsSuccess)
            {
                return this.ToApiResult<string>(result);
            }
            return ApiResult<string>.Ok("Success Updated");
        }
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeleteDestination(int Id)
        {
            var result = await _destinationService.DeleteAsync(Id);
            if (!result.IsSuccess)
            {
                return this.ToApiResult<string>(result);
            }
            return ApiResult<string>.Ok("Success Deleted");
        }
        [HttpPost("{DestinationId}/Activity/{ActivityId}")]
        public async Task<ApiResult<string>> AddDestinationActivity(int DestinationId , int ActivityId)
        {
            var result = await _destinationService.AddDestinationActivity(DestinationId,ActivityId);
            if (!result.IsSuccess)
            {
                return this.ToApiResult<string>(result);
            }
            return ApiResult<string>.Created(result.Message);
        }
        [HttpGet("{destinationId}/Activities")]
        public async Task<ApiResult<GetDestinationActivitiesByIdDto>> GetDestinationActivities(int destinationId)
        {
            var result = await _destinationService.GetDestinationActivitiesByIdDto(destinationId);
            if (!result.IsSuccess)
            {
                return this.ToApiResult<GetDestinationActivitiesByIdDto>(result);
            }
            return ApiResult<GetDestinationActivitiesByIdDto>.Ok(result.Value!);
        }
        
        [HttpDelete("{DestinationId}/Activity/{ActivityId}")]
        public async Task<ApiResult<string>> DeleteDestinationActivity(int DestinationId, int ActivityId)
        {
            var result = await _destinationService.DeleteDestinationActivity(DestinationId, ActivityId);
            if (!result.IsSuccess)
            {
                return this.ToApiResult<string>(result);
            }
            return ApiResult<string>.Ok("Success Deleted");
        }

    }
}
