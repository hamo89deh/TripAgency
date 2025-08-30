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
    }
}
