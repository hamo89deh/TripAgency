using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.Phobia.Queries;
using TripAgency.Service.Feature.Phobia.Commands;


namespace TripAgency.Service.Abstracts
{
    public interface IPhobiaService : IReadService<Phobia, GetPhobiaByIdDto, GetPhobiasDto>,
                                      IUpdateService<Phobia, UpdatePhobiaDto>,
                                      IAddService<Phobia, AddPhobiaDto, GetPhobiaByIdDto>,
                                      IDeleteService<Phobia>
    {
    }
}
