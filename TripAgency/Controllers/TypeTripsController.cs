using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;

namespace TripAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeTripsController : ControllerBase
    {
        public TypeTripsController(ITypeTripService typeTripService, IMapper mapper)
        {
            _typeTripService = typeTripService;
            _mapper = mapper;
        }

        public ITypeTripService _typeTripService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetTypeTripsDto>>> GetTypeTrips()
        {
            var typetripsResult = await _typeTripService.GetAllAsync();
            if (!typetripsResult.IsSuccess)
                return this.ToApiResult(typetripsResult);
            return ApiResult<IEnumerable<GetTypeTripsDto>>.Ok(typetripsResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetTypeTripByIdDto>> GetTypeTripById(int id)
        {
            var typetripResult = await _typeTripService.GetByIdAsync(id);
            if (!typetripResult.IsSuccess)
                return this.ToApiResult(typetripResult);
            return ApiResult<GetTypeTripByIdDto>.Ok(typetripResult.Value!);
        }

        [HttpGet("Name/{name}")]
        public async Task<ApiResult<GetTypeTripByIdDto>> GetTypeTripByName(string name)
        {
            var typetripResult = await _typeTripService.GetTypeTripByNameAsync(name);
            if (!typetripResult.IsSuccess)
                return this.ToApiResult(typetripResult);
            return ApiResult<GetTypeTripByIdDto>.Ok(typetripResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<GetTypeTripByIdDto>> AddTypeTrip(AddTypeTripDto typetrip)
        {
            var typetripResult = await _typeTripService.CreateAsync(typetrip);
            if (!typetripResult.IsSuccess)
            {
                return this.ToApiResult(typetripResult);
            }
            return ApiResult<GetTypeTripByIdDto>.Created(typetripResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateTypeTrip(UpdateTypeTripDto updateTypeTrip)
        {
            var typetripResult = await _typeTripService.UpdateAsync(updateTypeTrip.Id, updateTypeTrip);
            if (!typetripResult.IsSuccess)
                return this.ToApiResult<string>(typetripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteTypeTrip(int id)
        {
            var typetripResult = await _typeTripService.DeleteAsync(id);
            if (!typetripResult.IsSuccess)
                return this.ToApiResult<string>(typetripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
