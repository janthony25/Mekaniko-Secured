using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public required string IssueName { get; set; }
        public string? PaymentTerm { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool? IsPaid { get; set; }

        // FK to Car
        public int CarId { get; set; }
        public Car Car { get; set; }

        //1-to-M Invoice-InvoiceItem
        public ICollection<InvoiceItem> InvoiceItem { get; set; }
    }
}
