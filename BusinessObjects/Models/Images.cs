namespace BusinessObjects.Models
{
    public class Images
    {
        public string Id { get; set; }
        public string ImagesUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
