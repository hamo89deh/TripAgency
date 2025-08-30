using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Context;
using TripAgency.Service.Abstracts;

namespace TripAgency.Service.Implemetations
{
    public class MediaService : IMediaService
    {
        private readonly TripAgencyDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MediaService> _logger;

        public MediaService(TripAgencyDbContext context, IWebHostEnvironment environment, ILogger<MediaService> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<Media> UploadMediaAsync(IFormFile file, string altText = null)
        {
            try
            {
                // إنشاء اسم فريد للملف
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var uploadPath = Path.Combine("uploads", "media");
                var fullPath = Path.Combine(_environment.WebRootPath, uploadPath);

                // إنشاء المجلد إذا لم يكن موجوداً
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                var filePath = Path.Combine(fullPath, fileName);

                // حفظ الملف
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // حفظ المعلومات في قاعدة البيانات
                var media = new Media
                {
                    FileName = fileName,
                    FilePath = filePath,
                    PublicUrl = $"/uploads/media/{fileName}",
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    AltText = altText
                };

                _context.Medias.Add(media);
                await _context.SaveChangesAsync();

                return media;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media");
                throw;
            }
        }

        //public async Task<bool> AssociateWithPackageTripAsync(Guid mediaId, Guid packageTripId, int displayOrder = 0, bool isMain = false, string customAltText = null)
        //{
        //    try
        //    {
        //        // التحقق من وجود العلاقة مسبقاً
        //        var existingAssociation = await _context.PackageTripMedias
        //            .FirstOrDefaultAsync(x => x.MediaId == mediaId && x.PackageTripId == packageTripId);

        //        if (existingAssociation != null)
        //            return true; // العلاقة موجودة مسبقاً

        //        var association = new PackageTripMedia
        //        {
        //            PackageTripId = packageTripId,
        //            MediaId = mediaId,
        //            DisplayOrder = displayOrder,
        //            IsMain = isMain,
        //            CustomAltText = customAltText
        //        };

        //        _context.PackageTripMedias.Add(association);
        //        await _context.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error associating media with package trip");
        //        return false;
        //    }
        //}

        //public async Task<bool> AssociateWithDestinationAsync(Guid mediaId, Guid destinationId, int displayOrder = 0, bool isMain = false, string customAltText = null)
        //{
        //    try
        //    {
        //        // التحقق من وجود العلاقة مسبقاً
        //        var existingAssociation = await _context.DestinationMedias
        //            .FirstOrDefaultAsync(x => x.MediaId == mediaId && x.DestinationId == destinationId);

        //        if (existingAssociation != null)
        //            return true;

        //        var association = new DestinationMedia
        //        {
        //            DestinationId = destinationId,
        //            MediaId = mediaId,
        //            DisplayOrder = displayOrder,
        //            IsMain = isMain,
        //            CustomAltText = customAltText
        //        };

        //        _context.DestinationMedias.Add(association);
        //        await _context.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error associating media with destination");
        //        return false;
        //    }
        //}

        //public async Task<List<Media>> GetMediaByPackageTripAsync(Guid packageTripId)
        //{
        //    return await _context.PackageTripMedias
        //        .Where(x => x.PackageTripId == packageTripId)
        //        .Include(x => x.Media)
        //        .OrderBy(x => x.DisplayOrder)
        //        .Select(x => x.Media)
        //        .ToListAsync();
        //}

        //public async Task<List<Media>> GetMediaByDestinationAsync(Guid destinationId)
        //{
        //    return await _context.DestinationMedias
        //        .Where(x => x.DestinationId == destinationId)
        //        .Include(x => x.Media)
        //        .OrderBy(x => x.DisplayOrder)
        //        .Select(x => x.Media)
        //        .ToListAsync();
        //}

        // دوال Disassociate مشابهة...
    }
}
