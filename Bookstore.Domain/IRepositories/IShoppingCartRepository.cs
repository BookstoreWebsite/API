using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IShoppingCartRepository
    {
        Task AddProductAsync(Guid shoppingCartUserId, Guid productId);
        Task<List<CartItem>> GetAllCartItemsAsync(Guid shoppingCartUserId);
        Task<Product> GetByProductIdAsync(Guid id);
        Task<ShoppingCart> GetByShoppingCartUserIdAsync(Guid id);
        Task CreatePurchaseAsync(Guid readerId, Address address);
        Task ClearShoppingCartAsync(Guid shoppingCartuserId);
        Task<CartItem> GetCartItemById(Guid itemId);
        Task IncrementItemQuantityAsync(Guid itemId);
        Task DecrementItemQuantityAsync(Guid itemId);
        Task RemoveItemAsync(Guid itemId);
    }
}
