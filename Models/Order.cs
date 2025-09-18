using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class Order : BaseEntity
    {
        public int BuyerId { get; set; }
        public int AddressId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Assigned, OutForDelivery, Delivered, Cancelled

        public int? DeliveryPartnerId { get; set; }

        // Navigation Properties
        public virtual User Buyer { get; set; } = null!;
        public virtual DeliveryAddress Address { get; set; } = null!;
        public virtual User? DeliveryPartner { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual Payment? Payment { get; set; }
    }
}