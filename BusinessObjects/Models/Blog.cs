namespace BusinessObjects.Models
{
    public class Blog
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }
    }
}
