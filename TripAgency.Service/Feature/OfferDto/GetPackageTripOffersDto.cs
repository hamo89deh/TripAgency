namespace TripAgency.Service.Feature.PromotionDto
{
    public class GetPackageTripOffersDto
    {
        public int OfferId { get; set; }
        public string OfferName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApply { get; set; }
    }
}
