using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public Guid ReaderId { get; set; }
        public User Reader { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
