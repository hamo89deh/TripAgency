namespace TripAgency.Service.Feature.PromotionDto
{
    public class GetOfferByIdDto
    {
        public int Id { get; set; }
        public string OfferName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
