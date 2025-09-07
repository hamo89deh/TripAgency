using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.PromotionDto;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Trip.Queries;


namespace TripAgency.Service.Abstracts
{
    public interface IOfferService :IReadService<Offer, GetOfferByIdDto, GetOffersDto>,
                                        IUpdateService<Offer, UpdateOfferDto>,
                                        IAddService<Offer, AddOfferDto, GetOfferByIdDto>,
                                        IDeleteService<Offer>
    {
     
    }


}
