using Microsoft.AspNetCore.Http;
using TripAgency.Data.Entities;

namespace TripAgency.Service.Abstracts
{
    public interface IMediaService
    {
        Task<string> UploadMediaAsync(string Location,IFormFile file);
        Task<bool> DeleteMediaAsync(string imageUrl);
    }
}
