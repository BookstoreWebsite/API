using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class Book : Product
    {
        public string Author { get; set; }
        public double? Rating { get; set; }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<User> WishedBy { get; set; } = new List<User>();
        public ICollection<User> ReadBy { get; set; } = new List<User>();
    }
}
