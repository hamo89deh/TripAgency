using Microsoft.AspNetCore.Http;
using TripAgency.Data.Entities;

namespace TripAgency.Service.Abstracts
{
    public interface IMediaService
    {
        Task<Media> UploadMediaAsync(IFormFile file, string altText = null);
        //Task<bool> AssociateWithPackageTripAsync(Guid mediaId, Guid packageTripId, int displayOrder = 0, bool isMain = false, string customAltText = null);
        //Task<bool> AssociateWithDestinationAsync(Guid mediaId, Guid destinationId, int displayOrder = 0, bool isMain = false, string customAltText = null);
        //Task<bool> DisassociateFromPackageTripAsync(Guid mediaId, Guid packageTripId);
        //Task<bool> DisassociateFromDestinationAsync(Guid mediaId, Guid destinationId);
        //Task<List<Media>> GetMediaByPackageTripAsync(Guid packageTripId);
        //Task<List<Media>> GetMediaByDestinationAsync(Guid destinationId);
    }
}
