using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = null!;
        public string SmtpPass { get; set; } = null!;
        public string FromEmail { get; set; } = null!;
        public string FromName { get; set; } = null!;
    }
}
