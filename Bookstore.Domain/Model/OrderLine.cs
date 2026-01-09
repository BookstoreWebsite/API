using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class OrderLine
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public byte[]? ProductImageBytes { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
