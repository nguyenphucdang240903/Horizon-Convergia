namespace BusinessObjects.Models
{
    public class Blog
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CategoryId { get; set; } 
        public Category Category { get; set; }
    }
}
