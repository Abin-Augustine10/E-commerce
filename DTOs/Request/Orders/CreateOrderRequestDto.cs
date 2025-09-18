using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Orders
{
    public class CreateOrderRequestDto
    {
        [Required]
        public int AddressId { get; set; }
    }

    public class UpdateOrderStatusRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}