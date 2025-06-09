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
            CreateMap<AddPackageTripDestinationActivityDto, PackageTripDestinationActivity>()
                .ForMember(d => d.PackageTripDestinationId, op => op.MapFrom(s => s.PackageTripDestinationId));
                
        }
    }
}
