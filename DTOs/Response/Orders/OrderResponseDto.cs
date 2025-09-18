using ShopZone.DTOs.Response.Address;
using ShopZone.DTOs;

namespace ShopZone.DTOs.Response.Orders
{
    public class OrderResponseDto : BaseDto<int>
    {
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public AddressResponseDto DeliveryAddress { get; set; } = null!;
        public string? DeliveryPartnerName { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
        public PaymentResponseDto? Payment { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    public class PaymentResponseDto
    {
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }
}
