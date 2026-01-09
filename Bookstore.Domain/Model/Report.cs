using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class Report
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public ReportReason Reason { get; set; }
        public Guid ReaderId { get; set; }
        public User Reader { get; set; }
        public Guid? ReviewId { get; set; }
        public Review? Review { get; set; }
        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; }
    }
}
