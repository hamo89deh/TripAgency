namespace TripAgency.Service.Feature.OfferDto
{
    public class GetOffersDto 
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
