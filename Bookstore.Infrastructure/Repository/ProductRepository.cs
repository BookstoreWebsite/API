using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task AddNewPriceListEntryAsync(Guid productId, decimal price)
        {
            var oldPriceListEntry = await GetCurrentPriceListEntryAsync(productId);

            oldPriceListEntry.ValidTo = DateTime.UtcNow;
            var date = DateTime.UtcNow;

            PriceListEntry priceListEntry = new PriceListEntry
            {
                ProductId = productId,
                Price = price,
                ValidFrom = date
            };

            _context.PriceListEntries.Add(priceListEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<PriceListEntry> GetCurrentPriceListEntryAsync(Guid productId) 
        {
            var date = DateTime.UtcNow;

            var priceEntry = await _context.PriceListEntries
                .Where(e =>
                    e.ProductId == productId &&
                    e.ValidFrom <= date &&
                    (e.ValidTo == null || e.ValidTo > date))
                .OrderByDescending(e => e.ValidFrom)
                .FirstAsync();

            return priceEntry;
        }

        public async Task<Product> GetById(Guid id) 
        {
            return await _context.Products.Include(p => p.PriceListEntries).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
