using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;

namespace TripAgency.Service.Implemetations
{
    public class CityService : ICityService
    {
        private ICityRepositoryAsync _cityRepository {  get; set; }
        public IMapper _mapper { get; }

        public CityService(ICityRepositoryAsync cityRepository,
                           IMapper mapper
                           )
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }



        public async Task<Result<IEnumerable<GetCitiesDto>>> GetCitiesAsync()
        {
            var cities= await _cityRepository.GetTableNoTracking().ToListAsync();
            if (cities.Count == 0)
                return Result<IEnumerable<GetCitiesDto>>.NotFound(" not");
            var citiesResult = _mapper.Map<List<GetCitiesDto>>(cities);
            return Result<IEnumerable<GetCitiesDto>>.Success(citiesResult);

        }

        public async Task<Result<GetCityByIdDto>> GetCityByIdAsync(int id)
        {
            var city= await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c=>c.Id==id);
            if (city is null)
                return Result<GetCityByIdDto>.NotFound($"Not Found City with Id : {id}");
            var cityResult = _mapper.Map<GetCityByIdDto>(city);
            return Result<GetCityByIdDto>.Success(cityResult);
        }

        public async Task<Result> CreateCityAsync(AddCityDto addCityDto)
        {
            var mapCity = _mapper.Map<City>(addCityDto);
            await _cityRepository.AddAsync(mapCity);
            return Result.Success($"Success cityID with id : {mapCity.Id}");
        }

        public async Task<Result> UpdateCityAsync(EditCityDto updateCityDto)
        {
            var city = await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c => c.Id == updateCityDto.Id);
            if (city is null)
                return Result.NotFound($"Not Found City with Id : {updateCityDto.Id}");
            var mapCity = _mapper.Map<City>(updateCityDto);
            await _cityRepository.UpdateAsync(mapCity);
            return Result.Success();
        }

        public async Task<Result> DeleteCityAsync(int id)
        {
            var city = await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (city is null)
                return Result.NotFound($"Not Found City with Id : {id}");           
            await _cityRepository.DeleteAsync(city);
            return Result.Success();
        }

        public async Task<Result<GetCityByIdDto>> GetCityByNameAsync(string name)
        {
            var city = await _cityRepository.GetTableNoTracking().FirstOrDefaultAsync(c => c.Name == name);
            if (city is null)
                return Result<GetCityByIdDto>.NotFound($"Not Found City with Name : {name}");
            var cityResult = _mapper.Map<GetCityByIdDto>(city);
            return Result<GetCityByIdDto>.Success(cityResult);

        }
    }
}
