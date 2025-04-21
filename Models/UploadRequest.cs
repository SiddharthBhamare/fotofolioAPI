using Microsoft.AspNetCore.Mvc;

namespace fotofolioAPI.Models
{
    public class UploadRequest
    {
        public IFormFile? Image { get; set; }

        public string? Title { get; set; } = string.Empty;

        public string Category { get; set; }

        public string? YouTubeLink { get; set; } = string.Empty;
    }

}
