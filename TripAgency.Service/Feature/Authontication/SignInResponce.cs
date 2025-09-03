namespace TripAgency.Service.Feature.Authontication
{
    public class SignInResponce
    {
        public IEnumerable<GetRolesDto>GetRolesDto {  get; set; }
        public JwtAuthResult JwtAuthResult { get; set; }
    }

}
