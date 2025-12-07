using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.DTO
{
    public class BookDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; } 
    }
}
