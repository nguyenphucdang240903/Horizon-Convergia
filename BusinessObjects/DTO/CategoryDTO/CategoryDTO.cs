namespace BusinessObjects.DTO.CategoryDTO
{
    public class CategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ParentCategoryId { get; set; } 
    }
}
