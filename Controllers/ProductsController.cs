using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopZone.DTOs.Request.Products;
using ShopZone.DTOs.Response.Common;
using ShopZone.DTOs.Response.Products;
using ShopZone.Models;
using ShopZone.Services.Interfaces;
using System.Security.Claims;

namespace ShopZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<User> _userRepository;

        public ProductsController(
            IGenericRepository<Product> productRepository,
            IGenericRepository<User> userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ProductListResponseDto>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                var products = await _productRepository.GetPagedAsync(page, pageSize);
                var totalCount = await _productRepository.CountAsync();

                var productDtos = new List<ProductResponseDto>();
                foreach (var product in products)
                {
                    var seller = await _userRepository.GetByIdAsync(product.SellerId);
                    productDtos.Add(new ProductResponseDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        SellerId = product.SellerId,
                        SellerName = seller?.Name ?? "",
                        CreatedAt = product.CreatedAt
                    });
                }

                var response = new ProductListResponseDto
                {
                    Products = productDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    HasMore = page * pageSize < totalCount
                };

                return Ok(ApiResponse<ProductListResponseDto>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductListResponseDto>.ErrorResponse("Failed to get products"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductResponseDto>.ErrorResponse("Product not found"));
                }

                var seller = await _userRepository.GetByIdAsync(product.SellerId);
                var productDto = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    SellerId = product.SellerId,
                    SellerName = seller?.Name ?? "",
                    CreatedAt = product.CreatedAt
                };

                return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(productDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductResponseDto>.ErrorResponse("Failed to get product"));
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                var product = new Product
                {
                    SellerId = userId,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Stock = request.Stock
                };

                var createdProduct = await _productRepository.AddAsync(product);

                var seller = await _userRepository.GetByIdAsync(userId);
                var productDto = new ProductResponseDto
                {
                    Id = createdProduct.Id,
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    Price = createdProduct.Price,
                    Stock = createdProduct.Stock,
                    SellerId = createdProduct.SellerId,
                    SellerName = seller?.Name ?? "",
                    CreatedAt = createdProduct.CreatedAt
                };

                return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(productDto, "Product created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductResponseDto>.ErrorResponse("Failed to create product"));
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(int id, [FromBody] UpdateProductRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return NotFound(ApiResponse<ProductResponseDto>.ErrorResponse("Product not found"));
                }

                if (product.SellerId != userId)
                {
                    return Forbid("You can only update your own products");
                }

                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Stock = request.Stock;
                product.UpdatedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(product);

                var seller = await _userRepository.GetByIdAsync(userId);
                var productDto = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    SellerId = product.SellerId,
                    SellerName = seller?.Name ?? "",
                    CreatedAt = product.CreatedAt
                };

                return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(productDto, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductResponseDto>.ErrorResponse("Failed to update product"));
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse("Product not found"));
                }

                if (product.SellerId != userId)
                {
                    return Forbid("You can only delete your own products");
                }

                await _productRepository.DeleteAsync(id);

                return Ok(ApiResponse<string>.SuccessResponse("", "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Failed to delete product"));
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("my-products")]
        public async Task<ActionResult<ApiResponse<ProductListResponseDto>>> GetMyProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var products = await _productRepository.GetPagedAsync(page, pageSize, p => p.SellerId == userId);
                var totalCount = await _productRepository.CountAsync(p => p.SellerId == userId);

                var seller = await _userRepository.GetByIdAsync(userId);
                var productDtos = products.Select(product => new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    SellerId = product.SellerId,
                    SellerName = seller?.Name ?? "",
                    CreatedAt = product.CreatedAt
                }).ToList();

                var response = new ProductListResponseDto
                {
                    Products = productDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    HasMore = page * pageSize < totalCount
                };

                return Ok(ApiResponse<ProductListResponseDto>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductListResponseDto>.ErrorResponse("Failed to get products"));
            }
        }
    }
}
