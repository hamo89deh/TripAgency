using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Entities;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;

namespace TripAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        public CitiesController(ICityService cityService , IMapper mapper)
        {
            _cityService = cityService;
            _mapper = mapper;
        }

        public ICityService _cityService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetCitiesDto>>> GetCities()
        { 
            var citiesResult = await _cityService.GetCitiesAsync();
            if (!citiesResult.IsSuccess)
               return this.ToApiResult(citiesResult);         
            return ApiResult<IEnumerable<GetCitiesDto>>.Ok(citiesResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetCityByIdDto>> GetCityById(int id)
        {
            var cityResult = await _cityService.GetCityByIdAsync(id);
            if (!cityResult.IsSuccess)
                return this.ToApiResult(cityResult);
            return ApiResult<GetCityByIdDto>.Ok(cityResult.Value!);
        }

        [HttpGet("Name/{name}")]
        public async Task<ApiResult<GetCityByIdDto>> GetCityByName(string name)
        {
            var cityResult = await _cityService.GetCityByNameAsync(name);
            if (!cityResult.IsSuccess)
                return this.ToApiResult(cityResult);
            return ApiResult<GetCityByIdDto>.Ok(cityResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddCity(AddCityDto city)
        {
            var cityResult= await _cityService.CreateCityAsync(city);
            if (!cityResult.IsSuccess)
                return this.ToApiResult<string>(cityResult);
            return ApiResult<string>.Created(cityResult.Message);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateCity(EditCityDto updateCity)
        {
            var cityResult = await _cityService.UpdateCityAsync(updateCity);
            if (!cityResult.IsSuccess)
                return this.ToApiResult<string>(cityResult);
            return ApiResult<string>.NoContent("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteCity(int id)
        {
            var cityResult = await _cityService.DeleteCityAsync(id);
            if (!cityResult.IsSuccess)
                return this.ToApiResult<string>(cityResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
