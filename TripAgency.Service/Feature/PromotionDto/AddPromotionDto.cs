namespace TripAgency.Service.Feature.PromotionDto
{
    // DTOs
    public class AddPromotionDto
    {
        public int PackageTripId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
