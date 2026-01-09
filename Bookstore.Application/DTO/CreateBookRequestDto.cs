using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.DTO
{
    public class CreateBookRequestDto
    {
        public BookDto bookDto { get; set; }
        public List<Guid> genreIds { get; set; }
    }
}
