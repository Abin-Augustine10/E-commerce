using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopZone.DTOs.Request.Cart;
using ShopZone.DTOs.Response.Cart;
using ShopZone.DTOs.Response.Common;
using ShopZone.Models;
using ShopZone.Services.Interfaces;
using System.Security.Claims;

namespace ShopZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Buyer")]
    public class CartController : ControllerBase
    {
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public CartController(
            IGenericRepository<Cart> cartRepository,
            IGenericRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> GetCart()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var cartItems = await _cartRepository.GetAllAsync(c => c.BuyerId == userId);

                var cartItemDtos = new List<CartItemResponseDto>();
                foreach (var item in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        cartItemDtos.Add(new CartItemResponseDto
                        {
                            Id = item.Id,
                            ProductId = item.ProductId,
                            ProductName = product.Name,
                            ProductPrice = product.Price,
                            Quantity = item.Quantity,
                            CreatedAt = item.CreatedAt
                        });
                    }
                }

                var response = new CartResponseDto
                {
                    Items = cartItemDtos
                };

                return Ok(ApiResponse<CartResponseDto>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartResponseDto>.ErrorResponse("Failed to get cart"));
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<CartItemResponseDto>>> AddToCart([FromBody] AddToCartRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return NotFound(ApiResponse<CartItemResponseDto>.ErrorResponse("Product not found"));
                }

                if (product.Stock < request.Quantity)
                {
                    return BadRequest(ApiResponse<CartItemResponseDto>.ErrorResponse("Insufficient stock"));
                }

                var existingCartItem = await _cartRepository.GetAsync(c => c.BuyerId == userId && c.ProductId == request.ProductId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += request.Quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                    await _cartRepository.UpdateAsync(existingCartItem);

                    var updatedDto = new CartItemResponseDto
                    {
                        Id = existingCartItem.Id,
                        ProductId = existingCartItem.ProductId,
                        ProductName = product.Name,
                        ProductPrice = product.Price,
                        Quantity = existingCartItem.Quantity,
                        CreatedAt = existingCartItem.CreatedAt
                    };

                    return Ok(ApiResponse<CartItemResponseDto>.SuccessResponse(updatedDto, "Cart updated"));
                }

                var cartItem = new Cart
                {
                    BuyerId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                var createdItem = await _cartRepository.AddAsync(cartItem);

                var cartItemDto = new CartItemResponseDto
                {
                    Id = createdItem.Id,
                    ProductId = createdItem.ProductId,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = createdItem.Quantity,
                    CreatedAt = createdItem.CreatedAt
                };

                return Ok(ApiResponse<CartItemResponseDto>.SuccessResponse(cartItemDto, "Added to cart"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartItemResponseDto>.ErrorResponse("Failed to add to cart"));
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<ApiResponse<CartItemResponseDto>>> UpdateCartItem(int id, [FromBody] UpdateCartRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var cartItem = await _cartRepository.GetByIdAsync(id);

                if (cartItem == null || cartItem.BuyerId != userId)
                {
                    return NotFound(ApiResponse<CartItemResponseDto>.ErrorResponse("Cart item not found"));
                }

                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    return NotFound(ApiResponse<CartItemResponseDto>.ErrorResponse("Product not found"));
                }

                if (product.Stock < request.Quantity)
                {
                    return BadRequest(ApiResponse<CartItemResponseDto>.ErrorResponse("Insufficient stock"));
                }

                cartItem.Quantity = request.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cartItem);

                var cartItemDto = new CartItemResponseDto
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = cartItem.Quantity,
                    CreatedAt = cartItem.CreatedAt
                };

                return Ok(ApiResponse<CartItemResponseDto>.SuccessResponse(cartItemDto, "Cart item updated"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartItemResponseDto>.ErrorResponse("Failed to update cart item"));
            }
        }

        [HttpDelete("remove/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveFromCart(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var cartItem = await _cartRepository.GetByIdAsync(id);

                if (cartItem == null || cartItem.BuyerId != userId)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse("Cart item not found"));
                }

                await _cartRepository.DeleteAsync(id);

                return Ok(ApiResponse<string>.SuccessResponse("", "Item removed from cart"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Failed to remove from cart"));
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse<string>>> ClearCart()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var cartItems = await _cartRepository.GetAllAsync(c => c.BuyerId == userId);

                foreach (var item in cartItems)
                {
                    await _cartRepository.DeleteAsync(item);
                }

                return Ok(ApiResponse<string>.SuccessResponse("", "Cart cleared"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Failed to clear cart"));
            }
        }
    }
}