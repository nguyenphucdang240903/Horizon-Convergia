using BusinessObjects.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.BlogDTO
{
    public class CreateBlogDTO
    {
        public string Title { get; set; }
        public string CategoryId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class UpdateBlogDTO : CreateBlogDTO
    {
        public string Title { get; set; }
        public string CategoryId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
        public string AuthorId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
