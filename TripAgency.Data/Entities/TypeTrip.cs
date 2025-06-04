namespace TripAgency.Data.Entities
{
    public class TypeTrip
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<PackageTripType> PackageTripTypes { get; set;}
    }

}
