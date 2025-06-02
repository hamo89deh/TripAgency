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
    public interface ICityService : IReadService<City ,GetCityByIdDto , GetCitiesDto> ,
                                    IUpdateService<City, UpdateCityDto > , 
                                    IAddService<City , AddCityDto , GetCityByIdDto>,
                                    IDeleteService<City>
    {
        Task<Result<GetCityByIdDto>> GetCityByNameAsync(string name);      
       
        
    }
}
