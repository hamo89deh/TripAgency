using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;

namespace TripAgency.Service.Mapping.PackageTripDestinationActivity_Entity
{
    public partial class  PackageTripDestinationActivityProfile
    {
        public void AddPackageTripDestinationActivityMapping()
        {
            CreateMap<AddPackageTripDestinationActivity, PackageTripDestinationActivity>()
                .ForMember(d => d.PackageTripDestinationId, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.ActivityId, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.Price, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.StartTime, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.EndTime, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.OrderActivity, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.Duration, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.PackageTripDestinationId));
        }
    }
}
