using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Service.Feature.ActivityPhobia.Queries
{
    public class GetActivityPhobiasDto
    {
        public int ActivityId { get; set; }
        public List<GetPhobiasDto> PhobiasDtos { get; set; }
    }
}
