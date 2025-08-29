using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Hotel.Commands;

namespace TripAgency.Service.Mapping.Hotel_Entity
{
    public partial class HotelProfile
    {
        public void EditHotelMapping()
        {
            CreateMap<UpdateHotelDto, Hotel>().
               ForMember(d => d.Name, op => op.MapFrom(s => s.Name)).
               ForMember(d => d.Location, op => op.MapFrom(s => s.Location)).
               ForMember(d => d.Email, op => op.MapFrom(s => s.Email)).
               ForMember(d => d.CityId, op => op.MapFrom(s => s.CityId)).
               ForMember(d => d.MidPriceForOneNight, op => op.MapFrom(s => s.MidPriceForOneNight)).
               ForMember(d => d.Rate, op => op.MapFrom(s => s.Rate));
        }
    }
}
