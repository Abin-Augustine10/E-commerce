using ShopZone.DTOs;

namespace ShopZone.DTOs.Response.Address
{
    public class AddressResponseDto : BaseDto<int>
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string FullAddress => $"{Street}, {City}, {State} - {Pincode}";
    }
}