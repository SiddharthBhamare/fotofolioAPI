namespace fotofolioAPI.Models
{
    public class Profile
    {
        public IFormFile? ProfilePicture { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Bio { get; set; }
    }
}