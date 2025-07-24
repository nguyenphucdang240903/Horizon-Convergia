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
        public string CategoryId { get; set; } // dùng chung cho tất cả
        public List<BlogCreateItem> Blogs { get; set; } // danh sách blog
    }
    public class BlogCreateItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string AuthorId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
