namespace TripAgency.Service.Feature.OfferDto
{
    public class GetPackageTripOffersDto
    {
        public int OfferId { get; set; }
        public string OfferName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApply { get; set; }
    }
}
