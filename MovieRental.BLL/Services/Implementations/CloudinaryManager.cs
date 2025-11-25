using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MovieRental.BLL.Services.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class CloudinaryManager : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryManager(IConfiguration configuration)
        {
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var key = configuration["CloudinarySettings:Key"];
            var secret = configuration["CloudinarySettings:Secret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("Cloudinary settings not found in configuration");

            var account = new Account(cloudName, key, secret);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        #region Image Operations

        public async Task<string> ImageCreateAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}");

            // Validate file size (max 10MB for images)
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File size exceeds 10MB limit");

            string fileName = $"{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Transformation = new Transformation()
                    .Width(1920)
                    .Height(1080)
                    .Crop("limit")
                    .Quality("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<List<string>> ImageCreateAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("File list is empty or null");

            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var url = await ImageCreateAsync(file);
                    uploadedUrls.Add(url);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other files
                    Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                }
            }

            return uploadedUrls;
        }

        public async Task<bool> ImageDeleteAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            try
            {
                var publicId = GetPublicIdFromUrl(imageUrl);
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Video Operations

        public async Task<string> VideoUploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Video file is empty or null");

            // Validate video file type
            var allowedExtensions = new[] { ".mp4", ".avi", ".mov", ".mkv", ".webm" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"Invalid video type. Allowed: {string.Join(", ", allowedExtensions)}");

            // Validate file size (max 500MB for videos)
            if (file.Length > 500 * 1024 * 1024)
                throw new ArgumentException("Video size exceeds 500MB limit");

            string fileName = $"{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(fileName, stream),
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("mp4"),
                EagerAsync = true,
                EagerTransforms = new List<Transformation>
                {
                    new Transformation().Width(1920).Height(1080).Crop("limit"),
                    new Transformation().Width(1280).Height(720).Crop("limit"),
                    new Transformation().Width(640).Height(360).Crop("limit")
                }
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary video upload error: {uploadResult.Error.Message}");

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<bool> VideoDeleteAsync(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
                return false;

            try
            {
                var publicId = GetPublicIdFromUrl(videoUrl);
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Video
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Helper Methods

        public string GetPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');

                // Cloudinary URL format: .../upload/v123456789/public_id.extension
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex >= 0 && uploadIndex + 2 < segments.Length)
                {
                    var fileNameWithExtension = segments[uploadIndex + 2];
                    return Path.GetFileNameWithoutExtension(fileNameWithExtension);
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}