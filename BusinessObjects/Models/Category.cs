﻿namespace BusinessObjects.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
