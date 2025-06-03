namespace TripAgency.Service.Feature.PackageTrip.Commands
{
    public class UpdatePackagaTripDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CancellationPolicy { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Price { get; set; }
        public int TripId { get; set; }

    }
}
