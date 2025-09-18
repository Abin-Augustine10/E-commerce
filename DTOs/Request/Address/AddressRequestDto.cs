using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Address
{
    public class CreateAddressRequestDto
    {
        [Required, MaxLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }

    public class UpdateAddressRequestDto : CreateAddressRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}