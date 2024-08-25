using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
        public required decimal ItemPrice { get; set; }
        public decimal? ItemTotal { get; set; }

        // FK to Invoice 
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
