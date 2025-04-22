using System.ComponentModel.DataAnnotations;

namespace fotofolioAPI.Models
{
    public class UploadRequest
    {
        public IFormFile? Image { get; set; }

        public string? Title { get; set; } = string.Empty;

        public string Category { get; set; }

        public string? YouTubeLink { get; set; } = string.Empty;
    }
    public class MultipleUploadRequest
    {
        [Required]
        public IList<IFormFile> Images { get; set; }  // Changed from single image to list
        public string Category { get; set; }
    }
}
