using TripAgency.Data.Entities.Identity;


namespace TripAgency.Service.Abstracts
{
    public interface ICurrentUserService
    {
        public Task<User> GetUserAsync();
        public int GetUserId();
        public Task<List<string>> GetCurrentUserRolesAsync();
    }
}
