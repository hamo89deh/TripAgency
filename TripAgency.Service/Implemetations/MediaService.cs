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
                if (_environment.WebRootPath is null)
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
        // وظيفة جديدة لحذف الصورة بناءً على imageUrl
        public async Task<bool> DeleteMediaAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    _logger.LogWarning("imageUrl is null or empty");
                    return false;
                }

                // الحصول على المسار الكامل للملف
                var filePath = GetAbsolutePath(imageUrl);

                // التحقق من وجود الملف
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"File not found: {filePath}");
                    return false;
                }

                // حذف الملف
                File.Delete(filePath);
                _logger.LogInformation($"File deleted successfully: {filePath}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {imageUrl}");
                return false;
            }
        }

        // وظيفة مساعدة للحصول على المسار المطلق من URL
        private string GetAbsolutePath(string relativeUrl)
        {
            // إزالة البادئة "/" إذا كانت موجودة
            var cleanPath = relativeUrl.StartsWith("/")
                ? relativeUrl.Substring(1)
                : relativeUrl;

            // الجمع بين مسار wwwroot والمسار النسبي
            return Path.Combine(_environment.WebRootPath ?? GetOrCreateWwwRootPath(), cleanPath);
        }

    }
}
