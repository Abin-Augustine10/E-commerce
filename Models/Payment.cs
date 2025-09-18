using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Success, Failed

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Order Order { get; set; } = null!;
    }
}