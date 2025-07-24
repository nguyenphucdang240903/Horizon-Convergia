namespace BusinessObjects.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public string? ParentCategoryId { get; set; } // New
        public Category? ParentCategory { get; set; } // Navigation
        public ICollection<Category> SubCategories { get; set; } = new List<Category>(); // Navigation
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

        public ICollection<Product> Products { get; set; }
    }
}
