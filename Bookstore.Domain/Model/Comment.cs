using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid ReaderId { get; set; }
        public User Reader {  get; set; }
        public Guid ReviewId { get; set; }
        public Review Review { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public bool HasReplies { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
