using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.TypeTrip_Entity
{
    public partial class TypeTripProfile : Profile
    {
        public TypeTripProfile()
        {
            GetTypeTripByIdMapping();
            GetTypeTripsMapping();
            AddTypeTripMapping();
            UpdateTypeTripMapping();
        }
    }
}
