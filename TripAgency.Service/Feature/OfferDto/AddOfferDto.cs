namespace TripAgency.Service.Feature.OfferDto
{
    // DTOs
    public class AddOfferDto
    {
        public string OfferName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
 
}
