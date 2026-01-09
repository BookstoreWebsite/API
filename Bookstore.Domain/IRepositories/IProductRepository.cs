using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IProductRepository
    {
        Task<PriceListEntry> GetCurrentPriceListEntryAsync(Guid productId);
        Task AddNewPriceListEntryAsync(Guid productId, decimal price);
        Task<Product> GetById(Guid id);
    }
}
