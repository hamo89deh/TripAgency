namespace TripAgency.Service.Feature.PromotionDto
{
    public class GetPromotionsDto 
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
