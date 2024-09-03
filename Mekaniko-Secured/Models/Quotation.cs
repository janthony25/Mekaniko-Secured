using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class Quotation
    {
        [Key]
        public int QuotationId { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public required string IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? IsEmailSent { get; set; } // will mark the email as sent

        // FK to Car
        public int CarId { get; set; }
        public Car Car { get; set; }

        //1-to-M Invoice-InvoiceItem
        public ICollection<QuotationItem> QuotationItem { get; set; }
    }
}
