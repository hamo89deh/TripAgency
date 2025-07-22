using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Hotel.Commands;
using TripAgency.Service.Feature.Hotel.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IHotelService : IGenericService<Hotel , GetHotelByIdDto ,GetHotelsDto,AddHotelDto , UpdateHotelDto>
    {
        public Task<Result<GetHotelByIdDto>> GetHotelByNameAsync(string name);
    }
}
