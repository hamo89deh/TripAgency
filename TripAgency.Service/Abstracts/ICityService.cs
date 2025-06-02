using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;
using TripAgency.Infrastructure.Configurations;
using TripAgency.Service.Generic;


namespace TripAgency.Service.Abstracts
{
    public interface ICityService : IReadAndDeleteService<City ,GetCityByIdDto , GetCitiesDto>
    {
        Task<Result<GetCityByIdDto>> GetCityByNameAsync(string name);      
        Task<Result> CreateCityAsync(AddCityDto addCityDto);
        Task<Result> UpdateCityAsync(EditCityDto updateCityDto);    
    }
    public interface IHotelService
    {
        //Task<Result<IEnumerable<GetHotelsDto>>> GetAllHotelsAsync();
        //Task<Result<HotelDto>> GetHotelByIdAsync(int id);
        //Task<Result<HotelDto>> CreateHotelAsync(CreateHotelDto createHotelDto);
        //Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateHotelDto);
        //Task<Result> DeleteHotelAsync(int id);
        //public Task<City> GetCityByNameAsync(string name);
    }
}
