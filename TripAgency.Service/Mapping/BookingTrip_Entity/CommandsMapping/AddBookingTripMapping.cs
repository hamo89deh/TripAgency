using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.BookingTrip.Commands;

namespace TripAgency.Service.Mapping.BookingTrip_Entity
{
    public partial class BookingTripProfile
    {
        public void AddBookingTripMapping()
        {
            CreateMap<AddBookingTripDto, BookingTrip>()
                .ForMember(d => d.PackageTripDateId, op => op.MapFrom(s => s.TripDateId))
                .ForMember(d => d.UserId, op => op.MapFrom(s => s.UserId))
                .ForMember(d => d.PassengerCount, op => op.MapFrom(s => s.PassengerCount))
                .ForMember(d => d.ActualPrice, op => op.MapFrom(s => s.ActualPrice))
                .ForMember(d => d.Notes, op => op.MapFrom(s => s.Notes));

        }
    }
}
