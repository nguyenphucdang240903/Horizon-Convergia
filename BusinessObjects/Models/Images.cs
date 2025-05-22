using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Images
    {
        public long Id { get; set; }
        public string ImagesUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }
    }
}
