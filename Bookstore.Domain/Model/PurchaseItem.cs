using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class PurchaseItem
    {
        public Guid Id { get; set; }

        public Guid PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
    }
}
