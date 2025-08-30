using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;

namespace TripAgency.Service.Implemetations
{
    public class MediaService : IMediaService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MediaService> _logger;

        public MediaService(IWebHostEnvironment environment, ILogger<MediaService> logger)
        {

            _environment = environment;
            _logger = logger;
        }

        public string GetOrCreateWwwRootPath()
        {
            // إذا كان WebRootPath موجوداً، استخدمه
            if (!string.IsNullOrEmpty(_environment.WebRootPath))
            {
                return _environment.WebRootPath;
            }

            // إذا لم يكن موجوداً، قم بإنشائه في ContentRootPath
            var wwwRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");

            if (!Directory.Exists(wwwRootPath))
            {
                Directory.CreateDirectory(wwwRootPath);
                Console.WriteLine($"تم إنشاء مجلد wwwroot في: {wwwRootPath}");
            }

            return wwwRootPath;
        }

        public async Task<string> UploadMediaAsync(string Location, IFormFile file)
        {
            try
            {
                if(_environment.WebRootPath is null)
                    GetOrCreateWwwRootPath();
                // إنشاء اسم فريد للملف
                var fullPath = Path.Combine(_environment.WebRootPath!, Location);
                
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                // إنشاء المجلد إذا لم يكن موجوداً
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                var filePath = Path.Combine(fullPath, fileName);

                // حفظ الملف
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync();
                }
                return $"/{Location}/{fileName}";
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
