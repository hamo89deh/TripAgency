using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Service.Feature.TripDate.Commands;

namespace TripAgency.Service.Mapping.TripDate_Entity
{
    public partial class TripDateProfile
    {
        public void AddTripDateMapping()
        {
            CreateMap<AddPackageTripDateDto, PackageTripDate>()
                .ForMember(d => d.StartBookingDate, op => op.MapFrom(s => s.StartBookingDate))
                .ForMember(d => d.EndBookingDate, op => op.MapFrom(s => s.EndBookingDate))
                .ForMember(d => d.StartPackageTripDate, op => op.MapFrom(s => s.StartPackageTripDate))
                .ForMember(d => d.EndPackageTripDate, op => op.MapFrom(s => s.EndPackageTripDate))
                .ForMember(d => d.CreateDate, op => op.MapFrom(s => DateTime.Now))
                .ForMember(d => d.PackageTripId, op => op.MapFrom(s => s.PackageTripId))
                .ForMember(d => d.IsAvailable, op => op.MapFrom(s => false))
                .ForMember(d => d.Status, op => op.MapFrom(s => PackageTripDataStatus.Draft)) ;
        }
    }
}
