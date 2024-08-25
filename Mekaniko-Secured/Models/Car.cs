using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }
        public string CarRego { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public bool? CarPaymentStatus { get; set; }

        // FK to Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        // M-to-M Car-Make
        public List<CarMake> CarMake { get; set; }

        // 1-to-M Car-Invoice
        public ICollection<Invoice> Invoice { get; set; }
    }
}
