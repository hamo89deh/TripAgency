using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.Trip_Entity
{
    public partial class TripProfile : Profile
    {
        public TripProfile()
        {
            GetTripByIdMapping();
            GetTripsMapping();
            AddTripMapping();
            UpdateTripMapping();
        }
    }
}
