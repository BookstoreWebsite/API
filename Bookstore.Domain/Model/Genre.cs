using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Book>? Books { get; set; } = new List<Book>();
    }
}
