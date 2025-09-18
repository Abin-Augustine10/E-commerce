using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Cart
{
    public class AddToCartRequestDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}