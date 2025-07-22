using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Feature.Activity.Queries;

namespace TripAgency.Service.Feature.DestinationActivity.Queries
{
    public class GetDestinationActivitiesByIdDto
    {
        public int DestinationId { get; set; }
        public IEnumerable<GetActivityByIdDto> ActivitiesDto { get; set; }

    }
}
