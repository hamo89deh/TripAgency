using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TripAgency.Bases;
using TripAgency.Data.Entities;
using TripAgency.Feature.City;
using TripAgency.Feature.City.Command;
using TripAgency.Feature.City.Queries;
using TripAgency.Service.Abstracts;

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
            var Cities = await _cityService.GetAll().ToListAsync();
            if (Cities.Count == 0)
                return ApiResult <IEnumerable<GetCitiesDto>>.NotFound();
            var citiesResult = _mapper.Map<List<GetCitiesDto>>(Cities);
            return ApiResult < IEnumerable < GetCitiesDto >>.Ok(citiesResult);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetCityByIdDto>> GetCityById(int id)
        {
            var city = await _cityService.GetByIdAsync(id);
            if (city is null)
                return ApiResult<GetCityByIdDto>.NotFound();

            var cityResult = _mapper.Map<GetCityByIdDto>(city);

            return ApiResult<GetCityByIdDto>.Ok(cityResult);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddCity(AddCityDto city)
        {
            var ResultCity= await _cityService.AddAsync(_mapper.Map<City>(city));
            return ApiResult<string>.Ok($"CityId : {ResultCity.Id}");
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateCity(EditCityDto city)
        {
            var cityResult = await _cityService.GetAll().FirstOrDefaultAsync(x => x.Id == city.Id);
            if (cityResult is null)
                return ApiResult<string>.NotFound();
            await _cityService.UpdateAsync(_mapper.Map<City>(city));
            return ApiResult<string>.Ok("","Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteCity(int id)
        {
            var city = await _cityService.GetAll().FirstOrDefaultAsync(x=>x.Id==id);
            if (city is null)
                return ApiResult<string>.NotFound();

            await _cityService.DeleteAsync(city);
            return ApiResult<string>.Ok("","Success Deleted");
        }
    }
}
