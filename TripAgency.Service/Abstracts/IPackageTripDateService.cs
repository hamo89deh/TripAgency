using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripDateService : IReadService<PackageTripDate , GetPackageTripDateByIdDto , GetPackageTripDatesDto> , 
                                        IAddService<PackageTripDate , AddPackageTripDateDto , GetPackageTripDateByIdDto>
    {
       public Task<Result> UpdateStatusTripDate(UpdatePackageTripDateDto updateTripDateDto);
       public bool CanChangeStatus(PackageTripDateStatus currentStatus, PackageTripDateStatus newStatus);
       public Task<Result<IEnumerable<GetPackageTripDateByIdDto>>> GetDateForPackageTrip(int packageTripId, PackageTripDateStatus? newStatus);
    }
}
