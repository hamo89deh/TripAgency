using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.ActivityPhobia.Queries;

namespace TripAgency.Service.Abstracts
{
    public interface IActivityPhobiasService
    {
        Task<Result> AddActivityPhobias(AddActivityPhobiasDto addActivityPhobiasDto);
        Task<Result<GetActivityPhobiasDto>> GetActivityPhobiasAsync(int ActivityId);
        Task<Result> DeleteActivityPhobia(int ActivityId, int PhobiaId);
    }
}
