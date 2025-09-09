using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        public TypeTripsController(ITripTypeService typeTripService, IMapper mapper)
        {
            _typeTripService = typeTripService;
            _mapper = mapper;
        }

        public ITripTypeService _typeTripService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetTripTypesDto>>> GetTypeTrips()
        {
            var typetripsResult = await _typeTripService.GetAllAsync();
            if (!typetripsResult.IsSuccess)
                return this.ToApiResult(typetripsResult);
            return ApiResult<IEnumerable<GetTripTypesDto>>.Ok(typetripsResult.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetTypeTripByIdDto>> GetTypeTripById(int Id)
        {
            var typetripResult = await _typeTripService.GetByIdAsync(Id);
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
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<GetTypeTripByIdDto>> AddTypeTrip(AddTypeTripDto typetrip)
        {
            var typetripResult = await _typeTripService.CreateAsync(typetrip);
            if (!typetripResult.IsSuccess)
            {
                return this.ToApiResult(typetripResult);
            }
            return ApiResult<GetTypeTripByIdDto>.Created(typetripResult.Value!);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{Id}")]
        public async Task<ApiResult<string>> UpdateTypeTrip(int Id, UpdateTypeTripDto updateTypeTrip)
        {
            var typetripResult = await _typeTripService.UpdateAsync(Id, updateTypeTrip);
            if (!typetripResult.IsSuccess)
                return this.ToApiResult<string>(typetripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeleteTypeTrip(int Id)
        {
            var typetripResult = await _typeTripService.DeleteAsync(Id);
            if (!typetripResult.IsSuccess)
                return this.ToApiResult<string>(typetripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
