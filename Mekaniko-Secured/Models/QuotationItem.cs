using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class QuotationItem
    {
        [Key]
        public int QuotationItemId { get; set; }
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
        public required decimal ItemPrice { get; set; }
        public decimal? ItemTotal { get; set; }

        // FK to Invoice 
        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }
    }
}
