using ShopZone.DTOs;

namespace ShopZone.DTOs.Response.Products
{
    public class ProductResponseDto : BaseDto<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
    }

    public class ProductListResponseDto
    {
        public List<ProductResponseDto> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }
}