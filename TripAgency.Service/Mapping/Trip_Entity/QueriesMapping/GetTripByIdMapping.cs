using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Trip.Queries;

namespace TripAgency.Service.Mapping.Trip_Entity
{
    public partial class  TripProfile
    {
        public void GetTripByIdMapping()
        {
            CreateMap<Trip, GetTripByIdDto>().
                ForMember(d => d.Id, op => op.MapFrom(s => s.Id)).
                ForMember(d => d.Name, op => op.MapFrom(s => s.Name)).
                ForMember(d => d.ImageUrl, op => op.MapFrom(s => s.ImageUrl)).
                ForMember(d => d.Description, op => op.MapFrom(s => s.Description));
        }
    }
}
