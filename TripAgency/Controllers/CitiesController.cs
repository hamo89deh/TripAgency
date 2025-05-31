using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<GetCitiesDto>>> GetCities()
        {
            var Cities = await _cityService.GetAll().ToListAsync();
            if (Cities.Count == 0)
                return NotFound();
            var citiesResult = _mapper.Map<List<GetCitiesDto>>(Cities);
            return Ok(citiesResult);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCityByIdDto>> GetCityById(int id)
        {
            var city = await _cityService.GetByIdAsync(id);
            if (city is null)
                return NotFound();

            var citYResult = _mapper.Map<GetCityByIdDto>(city);

            return Ok(citYResult);
        }
        [HttpPost]
        public async Task<ActionResult<string>> AddCity(AddCityDto city)
        {
            var ResultCity= await _cityService.AddAsync(_mapper.Map<City>(city));
            return Ok($"id : {ResultCity.Id}");
        }
        [HttpPut]
        public async Task<ActionResult<string>> UpdateCity(EditCityDto city)
        {
            var cityResult = await _cityService.GetAll().FirstOrDefaultAsync(x => x.Id == city.Id);
            if (cityResult is null)
                return NotFound();
            await _cityService.UpdateAsync(_mapper.Map<City>(city));
            return Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteCity(int id)
        {
            var city = await _cityService.GetAll().FirstOrDefaultAsync(x=>x.Id==id);
            if (city is null)
                return NotFound();

            await _cityService.DeleteAsync(city);
            return Ok("Success Deleted");
        }
    }
}
