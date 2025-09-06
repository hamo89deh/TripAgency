namespace TripAgency.Service.Feature.PromotionDto
{
    // DTOs
    public class AddOfferDto
    {
        public string OfferName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
 
}
