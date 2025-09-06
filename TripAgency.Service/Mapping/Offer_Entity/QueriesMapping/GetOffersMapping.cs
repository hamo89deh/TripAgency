using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Service.Mapping.Promotioin_Entity
{
    public partial class OfferProfile
    {
        public void GetOffersMapping()
        {

            CreateMap<Offer, GetOfferByIdDto>().
                 ForMember(d => d.Id, op => op.MapFrom(s => s.Id)).
                 ForMember(d => d.EndDate, op => op.MapFrom(s => s.EndDate)).
                 ForMember(d => d.StartDate, op => op.MapFrom(s => s.StartDate)).
                 ForMember(d => d.DiscountPercentage, op => op.MapFrom(s => s.DiscountPercentage)).
                 ForMember(d => d.IsActive, op => op.MapFrom(s => s.IsActive));
        }
    }
}
