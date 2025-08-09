using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Service.Feature.PackageTripType.Commands
{
    public class AddPackageTripTypesDto
    {
        public int PackageTripId { get; set; }
        public List<int> TripTypes { get; set; }

    }
}
