using Microsoft.AspNetCore.Mvc;

namespace fotofolioAPI.Models
{
    public class UploadRequest
    {
        [FromForm]
        public IFormFile? Image { get; set; }

        [FromForm]
        public string? Title { get; set; }

        [FromForm]
        public string Category { get; set; }

        [FromForm]
        public string? YouTubeLink { get; set; }
    }

}
