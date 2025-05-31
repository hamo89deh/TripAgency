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
        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        public ICityService _cityService { get; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCitiesDto>>> GetCities()
        {
            var Cities = await _cityService.GetAll().ToListAsync();
            if (Cities.Count == 0)
                return NotFound();
            return Ok(Cities);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCityByIdDto>> GetCityById(int id)
        {
            var City = await _cityService.GetByIdAsync(id);
            if (City is null)
                return NotFound();

            return Ok(new GetCityByIdDto(id)
            {
                Name = City.Name
            });
        }
        [HttpPost]
        public async Task<ActionResult<string>> AddCity(AddCityDto city)
        {
            var ResultCity= await _cityService.AddAsync(new City
            {
                Name = city.Name
            });
            return Ok($"id : {ResultCity.Id}");
        }
        [HttpPut]
        public async Task<ActionResult<string>> UpdateCity(EditCityDto city)
        {
            var cityResult = await _cityService.GetAll().FirstOrDefaultAsync(x => x.Id == city.Id);
            if (cityResult is null)
                return NotFound();
            await _cityService.UpdateAsync(new City
            {
                Id = city.Id,
                Name = city.Name
            });
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
