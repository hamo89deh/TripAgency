using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Feature.Activity.Queries;

namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Commands
{
    public class AddPackageTripDestinationActivityDto
    {
        public int PackageTripDestinationId { get; set; }
        public IEnumerable<PackageTripDestinationActivitiesDto> ActivitiesDtos{get; set;}
   
    }
    public class GetPackageTripDestinationActivitiesDto
    {
        public int PackageTripDestinationId { get; set; }
        public IEnumerable<GetActivityByIdDto> ActivitiesDtos { get; set; }

    }
}
