using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.DTO
{
    public class PurchaseDto
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
