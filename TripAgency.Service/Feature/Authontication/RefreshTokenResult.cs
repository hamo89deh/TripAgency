namespace TripAgency.Service.Feature.Authontication
{
    public class RefreshTokenResult
    {
        public string UserName { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
