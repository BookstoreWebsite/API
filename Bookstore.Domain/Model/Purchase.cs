using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public Guid ReaderId { get; set; }
        public User Reader {  get; set; }
        public DateTime DateTime { get; set; }
        public Address Address { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<PurchaseItem> Items { get; set;} = new List<PurchaseItem>();
    }
}
