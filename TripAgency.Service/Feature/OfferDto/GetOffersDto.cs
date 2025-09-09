namespace TripAgency.Service.Feature.OfferDto
{
    public class GetOffersDto 
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
