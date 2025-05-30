using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Dtos.City;
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
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var Cities = await _cityService.GetAll().ToListAsync();
            if (Cities.Count == 0)
                return NotFound();
            return Ok(Cities);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCityById(int id)
        {
            var City = await _cityService.GetByIdAsync(id);
            if (City is null)
                return NotFound();
            return Ok(City);
        }
        [HttpPost]
        public async Task<ActionResult<City>> AddCity(AddCityDto city)
        {
            var ResultCity= await _cityService.AddAsync(new City
            {
                Name = city.Name
            });
            return Ok(ResultCity);
        }
        [HttpPut]
        public async Task<ActionResult<City>> UpdateCity(EditCityDto city)
        {
            var cityResult = await _cityService.GetAll().FirstOrDefaultAsync(x => x.Id == city.Id);
            if (cityResult is null)
                return NotFound();
            await _cityService.UpdateAsync(new City
            {
                Id = city.Id,
                Name = city.Name
            });
            return Ok();

        }
        [HttpDelete]
        public async Task<ActionResult<City>> DeleteCity(int id)
        {
            var city = await _cityService.GetAll().FirstOrDefaultAsync(x=>x.Id==id);
            if (city is null)
                return NotFound();

            await _cityService.DeleteAsync(city);
            return Ok();
        }
    }
}
