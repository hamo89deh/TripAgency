using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;

namespace TripAgency.Service.Mapping.TypeTrip_Entity
{
    public partial class TypeTripProfile
    {
        public void AddTypeTripMapping()
        {
            CreateMap<AddTypeTripDto, TypeTrip>()
                .ForMember(s => s.Name, op => op.MapFrom(s => s.Name));
                
        }
    }
}
