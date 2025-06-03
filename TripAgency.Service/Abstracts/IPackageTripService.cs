using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripService : IGenericService  <PackageTrip , GetPackageTripByIdDto, GetPackageTripsDto ,AddPackageTripDto ,UpdatePackageTripDto>
    {
    }
}
