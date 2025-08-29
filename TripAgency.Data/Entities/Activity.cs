namespace TripAgency.Data.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<DestinationActivity> DestinationActivities { get; set; }
        public IEnumerable<PackageTripDestinationActivity> PackageTripDestinationActivities { get; set; }
        public IEnumerable<ActivityPhobias> ActivityPhobias { get; set; }

    }
    public class Media 
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string PublicUrl { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public string AltText { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // العلاقات
        public virtual ICollection<PackageTripMedia> PackageTripMedias { get; set; } = new HashSet<PackageTripMedia>();
        public virtual ICollection<DestinationMedia> DestinationMedias { get; set; } = new HashSet<DestinationMedia>();
    }
    public class PackageTripMedia 
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }
        public int MediaId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; }
        public string CustomAltText { get; set; }

        public virtual PackageTrip PackageTrip { get; set; }
        public virtual Media Media { get; set; }
    }
    public class DestinationMedia 
    {
        public int Id {  get; set; }
        public int DestinationId { get; set; }
        public int MediaId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; }
        public string CustomAltText { get; set; }

        public virtual Destination Destination { get; set; }
        public virtual Media Media { get; set; }
    }

}
