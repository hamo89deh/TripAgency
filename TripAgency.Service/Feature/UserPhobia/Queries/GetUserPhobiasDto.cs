using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Service.Feature.ActivityPhobia.Queries
{
    public class GetUserPhobiasDto
    {
        public int UserId { get; set; }
        public List<GetPhobiasDto> PhobiasDtos { get; set; }
    }
}
