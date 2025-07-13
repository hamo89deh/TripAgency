using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripService : IGenericService  <PackageTrip , GetPackageTripByIdDto, GetPackageTripsDto ,AddPackageTripDto ,UpdatePackageTripDto>
    {
       public Task<Result<GetPackageTripDestinationsActivitiesDatesDto>> GetPackageTripDestinationsActivitiesDates(int packageTripId , enPackageTripDataStatusDto status);

    }
}
