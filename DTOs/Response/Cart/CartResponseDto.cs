using ShopZone.DTOs;

namespace ShopZone.DTOs.Response.Cart
{
    public class CartItemResponseDto : BaseDto<int>
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity;
    }

    public class CartResponseDto
    {
        public List<CartItemResponseDto> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
        public int TotalItems => Items.Sum(x => x.Quantity);
    }
}