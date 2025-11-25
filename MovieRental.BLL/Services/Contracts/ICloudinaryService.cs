using Microsoft.AspNetCore.Http;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ICloudinaryService
    {
        Task<string> ImageCreateAsync(IFormFile file);
        Task<List<string>> ImageCreateAsync(List<IFormFile> files);
        Task<bool> ImageDeleteAsync(string imageUrl);
        Task<string> VideoUploadAsync(IFormFile file);
        Task<bool> VideoDeleteAsync(string videoUrl);
        string GetPublicIdFromUrl(string url);
    }
}