using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Products
{
    public class CreateProductRequestDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; } = 0;
    }

    public class UpdateProductRequestDto : CreateProductRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}