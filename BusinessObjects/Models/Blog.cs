using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
