using BusinessObjects.DTO.BlogDTO;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.ReviewDTO
{
    public class CreateReviewDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }
    }
    public class UpdateReviewDTO : CreateReviewDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }
    }
    public class DeleteReviewDTO : CreateReviewDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public bool IsDeleted { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }
    }
}
