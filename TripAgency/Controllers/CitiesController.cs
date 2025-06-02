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
            var citiesResult = await _cityService.GetAllAsync();
            if (!citiesResult.IsSuccess)
               return this.ToApiResult(citiesResult);         
            return ApiResult<IEnumerable<GetCitiesDto>>.Ok(citiesResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetCityByIdDto>> GetCityById(int id)
        {
            var cityResult = await _cityService.GetByIdAsync(id);
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
        public async Task<ApiResult<GetCityByIdDto>> AddCity(AddCityDto city)
        {
            var cityResult= await _cityService.CreateAsync(city);
            if (!cityResult.IsSuccess)
            {                
                return this.ToApiResult(cityResult);
            }
            return ApiResult<GetCityByIdDto>.Created(cityResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateCity(UpdateCityDto updateCity)
        {
            var cityResult = await _cityService.UpdateAsync(updateCity.Id,updateCity);
            if (!cityResult.IsSuccess)
                return this.ToApiResult<string>(cityResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteCity(int id)
        {
            var cityResult = await _cityService.DeleteAsync(id);
            if (!cityResult.IsSuccess)
                return this.ToApiResult<string>(cityResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
