using TripAgency.Data.Entities;
using TripAgency.Service.Feature.OfferDto;

namespace TripAgency.Service.Mapping.Offer_Entity
{
    public partial class OfferProfile
    {
        public void GetOffersMapping()
        {

            CreateMap<Offer, GetOffersDto>().
                 ForMember(d => d.Id, op => op.MapFrom(s => s.Id)).
                 ForMember(d => d.Name, op => op.MapFrom(s => s.Name)).
                 ForMember(d => d.EndDate, op => op.MapFrom(s => s.EndDate)).
                 ForMember(d => d.StartDate, op => op.MapFrom(s => s.StartDate)).
                 ForMember(d => d.DiscountPercentage, op => op.MapFrom(s => s.DiscountPercentage)).
                 ForMember(d => d.IsActive, op => op.MapFrom(s => s.IsActive));
        }
    }
}
