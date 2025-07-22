using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Feature.Trip.Queries;

namespace TripAgency.Service.Mapping.TripDate_Entity
{
    public partial class TripDateProfile :Profile
    {
        public TripDateProfile()
        {
            GetTripDateByIdMapping();
            GetTripDatesMapping();
            AddTripDateMapping();
        }
    }
}
