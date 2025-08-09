using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.ActivityPhobia.Queries;
using TripAgency.Service.Feature.PackageTripType.Commands;
using TripAgency.Service.Feature.PackageTripType.Queries;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripTypesService
    {
        Task<Result> AddPackageTripTypes(AddPackageTripTypesDto addPackageTripTypesDto);
        Task<Result<GetPackageTripTypesDto>> GetPackageTripTypesAsync(int PackageTripId);
        Task<Result> DeletePackageTripType(int PackageTripId, int TripTypeId);

    }
}
