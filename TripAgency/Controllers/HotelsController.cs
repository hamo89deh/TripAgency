using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;
using TripAgency.Service.Feature.Hotel.Commands;
using TripAgency.Service.Feature.Hotel.Queries;
using TripAgency.Service.Implemetations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        public HotelsController(IHotelService hotelService, IMapper mapper)
        {
            _hotelService = hotelService;
            _mapper = mapper;
        }

        public IHotelService _hotelService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetHotelsDto>>> GetHotels()
        {
            var hotelsResult = await _hotelService.GetAllAsync();
            if (!hotelsResult.IsSuccess)
                return this.ToApiResult(hotelsResult);
            return ApiResult<IEnumerable<GetHotelsDto>>.Ok(hotelsResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetHotelByIdDto>> GetHotelById(int id)
        {
            var hotelResult = await _hotelService.GetByIdAsync(id);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult(hotelResult);
            return ApiResult<GetHotelByIdDto>.Ok(hotelResult.Value!);
        }

        [HttpGet("Name/{name}")]
        public async Task<ApiResult<GetHotelByIdDto>> GetHotelByName(string name)
        {
            var hotelResult = await _hotelService.GetHotelByNameAsync(name);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult(hotelResult);
            return ApiResult<GetHotelByIdDto>.Ok(hotelResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<GetHotelByIdDto>> AddHotel(AddHotelDto hotel)
        {
            var hotelResult = await _hotelService.CreateAsync(hotel);
            if (!hotelResult.IsSuccess)
            {
                return this.ToApiResult(hotelResult);
            }
            return ApiResult<GetHotelByIdDto>.Created(hotelResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateHotel(UpdateHotelDto updateHotel)
        {
            var hotelResult = await _hotelService.UpdateAsync(updateHotel.Id, updateHotel);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult<string>(hotelResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteHotel(int id)
        {
            var hotelResult = await _hotelService.DeleteAsync(id);
            if (!hotelResult.IsSuccess)
                return this.ToApiResult<string>(hotelResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
