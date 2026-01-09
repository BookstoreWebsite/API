using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.DTO
{
    public class CommentDto
    {
        public Guid? Id { get; set; }
        public string Text { get; set; }
        public string? Username { get; set; }
        public string? ProfilePicture { get; set; }
        public bool HasReplies { get; set; }
    }
}
