using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.DTO
{
    public class ReportDto
    {
        public Guid? Id { get; set; }
        public string Text { get; set; }
        public ReportReason Reason { get; set; }
        public Guid? ReaderId { get; set; }
        public Guid? ReviewId { get; set; }
        public Guid? CommentId { get; set; }
        public bool? IsReviewReport { get; set; }
    }
}
