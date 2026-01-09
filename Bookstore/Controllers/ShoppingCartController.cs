using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService) 
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpPost("add/{shoppingCartUserId}/{productId}")]
        public async Task<IActionResult> AddProduct(Guid shoppingCartUserId, Guid productId) 
        {
            var result = await _shoppingCartService.AddProductAsync(shoppingCartUserId, productId);

            if (!result) 
            {
                return BadRequest("Could not add product to shopping cart");
            }

            return Ok(new {message = "Product added to shopping cart"});
        }

        [HttpGet("getAllCartItems/{shoppingCartUserId}")]
        public async Task<ActionResult<CartItemDto>> GetAllCartItems(Guid shoppingCartUserId)
        {
            var cartItemDtos = await _shoppingCartService.GetAllCartItemsAsync(shoppingCartUserId);
            return Ok(cartItemDtos);
        }

        [HttpPost("createPurchase/{readerId}")]
        public async Task<IActionResult> CreatePurchase(Guid readerId, [FromBody]AddressDto addressDto) 
        {
            var result = await _shoppingCartService.CreatePurchaseAsync(readerId, addressDto);
            if (!result) 
            {
                return BadRequest("Could not purchase selected items!");
            }
            return Ok(new {message = "Items successfully purchased!"});
        }

        [HttpDelete("clearShoppingCart/{shoppingCartUserId}")]
        public async Task<IActionResult> ClearShoppingCart(Guid shoppingCartUserId) 
        {
            var result = await _shoppingCartService.ClearShoppingCartAsync(shoppingCartUserId);

            if (!result)
            {
                return NotFound(new { message = "Items not found!" });
            }

            return Ok(new { message = "Shopping cart cleared successfully!" });
        }

        [HttpPut("incrementItemQuantity/{itemId}")]
        public async Task<IActionResult> IncrementItemQuantity(Guid itemId) 
        {
            var result = await _shoppingCartService.IncrementItemQuantityAsync(itemId);

            if (!result) 
            {
                return NotFound(new { message = "Item not found!" });
            }

            return Ok(new { message = "Increased quantity successfully!" });
        }

        [HttpPut("decrementItemQuantity/{itemId}")]
        public async Task<IActionResult> DecrementItemQuantity(Guid itemId)
        {
            var result = await _shoppingCartService.DecrementItemQuantityAsync(itemId);

            if (!result)
            {
                return NotFound(new { message = "Item not found!" });
            }

            return Ok(new { message = "Decreased quantity successfully!" });
        }

        [HttpDelete("removeItem/{itemId}")]
        public async Task<IActionResult> RemoveItem(Guid itemId)
        {
            var result = await _shoppingCartService.RemoveItemAsync(itemId);

            if (!result)
            {
                return NotFound(new { message = "Item not found!" });
            }

            return Ok(new { message = "Removed item successfully!" });
        }
    }
}
