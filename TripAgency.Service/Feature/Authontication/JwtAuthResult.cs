namespace TripAgency.Service.Feature.Authontication
{
    public class JwtAuthResult
    {
        public string AccessToken { get; set; }
        public RefreshTokenResult refreshToken { get; set; }
    }
}
