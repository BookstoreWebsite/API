using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _repository;

        public ShoppingCartService(IShoppingCartRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddProductAsync(Guid shoppingCartUserId, Guid productId)
        {
            await _repository.AddProductAsync(shoppingCartUserId, productId);
            return true;
        }

        public async Task<List<CartItemDto>> GetAllCartItemsAsync(Guid ShoppingCartUserId)
        {
            List<CartItemDto> cartItemDtos = new List<CartItemDto>();
            var cartItems = await _repository.GetAllCartItemsAsync(ShoppingCartUserId);
            foreach (var cartItem in cartItems) 
            {
                var cartItemDto = new CartItemDto
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    ProductImageUrl = cartItem.Product.ImageUrl,
                    ProductPrice = cartItem.UnitPrice,
                    Quantity = cartItem.Quantity
                };

                cartItemDtos.Add(cartItemDto);
            }
            return cartItemDtos;
        }

        public async Task<bool> CreatePurchaseAsync(Guid readerId, AddressDto addressDto) 
        {
            DateTime dateTime = DateTime.Now;

            var address = new Address
            {
                Country = addressDto.Country,
                City = addressDto.City,
                Street = addressDto.Street,
                Number = addressDto.Number,
                PostalCode = addressDto.PostalCode
            };
            
            await _repository.CreatePurchaseAsync(readerId, address);
            return true;
        }

        public async Task<bool> ClearShoppingCartAsync(Guid shoppingCartUserId) 
        {
            await _repository.ClearShoppingCartAsync(shoppingCartUserId);
            return true;
        }

        public async Task<bool> IncrementItemQuantityAsync(Guid itemId) 
        {
            await _repository.IncrementItemQuantityAsync(itemId);
            return true;
        }

        public async Task<bool> DecrementItemQuantityAsync(Guid itemId)
        {
            await _repository.DecrementItemQuantityAsync(itemId);
            return true;
        }

        public async Task<bool> RemoveItemAsync(Guid itemId)
        {
            await _repository.RemoveItemAsync(itemId);
            return true;
        }
    }
}
