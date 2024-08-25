using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public bool? PaymentStatus { get; set; }

        // 1-to-M Customer-Car
        public ICollection<Car> Car { get; set; }
    }
}
