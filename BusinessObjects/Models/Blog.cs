namespace BusinessObjects.Models
{
    public class Blog
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }

        public long AuthorId { get; set; }
        public User Author { get; set; }
    }
}
