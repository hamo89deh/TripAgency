using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Trip.Commands;

namespace TripAgency.Service.Mapping.Trip_Entity
{
    public partial class TripProfile
    {
        public void AddTripMapping()
        {
            CreateMap<AddTripDto, Trip>().
                    ForMember(d => d.Name, op => op.MapFrom(s => s.Name)).
                    ForMember(d => d.Description, op => op.MapFrom(s => s.Description)).
                    ForMember(d => d.TypeTripId, op => op.MapFrom(s => s.TypeTripId));

        }
    }
}
