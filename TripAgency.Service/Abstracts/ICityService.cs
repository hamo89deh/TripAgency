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


namespace TripAgency.Service.Abstracts
{
    public interface ICityService 
    {
        Task<Result<IEnumerable<GetCitiesDto>>> GetCitiesAsync();    
        Task<Result<GetCityByIdDto>> GetCityByIdAsync(int id);
        Task<Result<GetCityByIdDto>> GetCityByNameAsync(string name);      
        Task<Result> CreateCityAsync(AddCityDto addCityDto);
        Task<Result> UpdateCityAsync(EditCityDto updateCityDto);    
        Task<Result> DeleteCityAsync(int id);     
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
