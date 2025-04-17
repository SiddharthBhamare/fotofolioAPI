namespace fotofolioAPI.Entities
{
    public class RawData
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public byte[] Image { get; set; }
        public string YoutubeURL { get; set; }
    }
}
