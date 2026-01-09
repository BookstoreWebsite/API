using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.Model
{
    public class ShoppingCart
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
