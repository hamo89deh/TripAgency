using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Service.Mapping.Promotioin_Entity
{
    public partial class PromotionProfile
    {
        public void GetPromotionByIdMapping()
        {

            CreateMap<Promotion, GetPromotionsDto>().
                 ForMember(d => d.Id, op => op.MapFrom(s => s.Id)).
                 ForMember(d => d.PackageTripId, op => op.MapFrom(s => s.PackageTripId)).
                 ForMember(d => d.EndDate, op => op.MapFrom(s => s.EndDate)).
                 ForMember(d => d.StartDate, op => op.MapFrom(s => s.StartDate)).
                 ForMember(d => d.DiscountPercentage, op => op.MapFrom(s => s.DiscountPercentage)).
                 ForMember(d => d.IsActive, op => op.MapFrom(s => s.IsActive));
        }
    }
}
