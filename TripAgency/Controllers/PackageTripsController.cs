using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTripsController : ControllerBase
    {
        public PackageTripsController(IPackageTripService hotelService, IMapper mapper)
        {
            _hotelService = hotelService;
            _mapper = mapper;
        }

        public IPackageTripService _hotelService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetPackageTripsDto>>> GetPackageTrips()
        {
            var hotelsResult = await _hotelService.GetAllAsync();
            if (!hotelsResult.IsSuccess)
                return this.ToApiResult(hotelsResult);
            return ApiResult<IEnumerable<GetPackageTripsDto>>.Ok(hotelsResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetPackageTripByIdDto>> GetPackageTripById(int id)
        {
            var hotelResult = await _hotelService.GetByIdAsync(id);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult(hotelResult);
            return ApiResult<GetPackageTripByIdDto>.Ok(hotelResult.Value!);
        }

       
        [HttpPost]
        public async Task<ApiResult<GetPackageTripByIdDto>> AddPackageTrip(AddPackageTripDto hotel)
        {
            var hotelResult = await _hotelService.CreateAsync(hotel);
            if (!hotelResult.IsSuccess)
            {
                return this.ToApiResult(hotelResult);
            }
            return ApiResult<GetPackageTripByIdDto>.Created(hotelResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdatePackageTrip(UpdatePackageTripDto updatePackageTrip)
        {
            var hotelResult = await _hotelService.UpdateAsync(updatePackageTrip.Id, updatePackageTrip);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult<string>(hotelResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteCity(int id)
        {
            var hotelResult = await _hotelService.DeleteAsync(id);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult<string>(hotelResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
