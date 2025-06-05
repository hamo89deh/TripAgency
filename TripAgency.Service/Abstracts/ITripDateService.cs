using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface ITripDateService : IReadService<TripDate , GetTripDateByIdDto , GetTripDatesDto> , 
                                        IAddService<TripDate , AddTripDateDto , GetTripDateByIdDto>
    {
       // public Task<Result> UpdateStatusTripDate(UpdateTripDateDto updateTripDateDto);
    }
}
