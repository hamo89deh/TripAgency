namespace TripAgency.Service.Feature.PromotionDto
{
    public class GetOfferByIdDto
    {
        public int Id { get; set; }
        public string OfferName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
