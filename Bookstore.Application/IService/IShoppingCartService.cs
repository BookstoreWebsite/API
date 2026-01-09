using Bookstore.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.IService
{
    public interface IShoppingCartService
    {
        Task<bool> AddProductAsync(Guid shoppingCartUserId, Guid productId);
        Task<List<CartItemDto>> GetAllCartItemsAsync(Guid shoppingCartUserId);
        Task<bool> CreatePurchaseAsync(Guid readerId, AddressDto addressDto);
        Task<bool> ClearShoppingCartAsync(Guid shoppingCartUserId);
        Task<bool> IncrementItemQuantityAsync(Guid itemId);
        Task<bool> DecrementItemQuantityAsync(Guid itemId);
        Task<bool> RemoveItemAsync(Guid itemId);
    }
}
