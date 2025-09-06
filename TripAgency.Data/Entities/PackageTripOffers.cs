namespace TripAgency.Data.Entities
{
    public class PackageTripOffers
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }    
        public int OfferId { get; set; }    
        public bool IsApply { get; set; }


        // العلاقات
        public PackageTrip PackageTrip { get; set; }
        public Offer Offer { get; set; }
    }

}
