using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;

namespace TripAgency.Service.Mapping.TypeTrip_Entity
{
    public partial class TypeTripProfile
    {
        public void GetTypeTripsMapping()
        {
            CreateMap<TypeTrip, GetTypeTripsDto>()
                .ForMember(s => s.Id, op => op.MapFrom(s => s.Id))
                .ForMember(s => s.Name, op => op.MapFrom(s => s.Name));
        }
    }
}
