using Microsoft.AspNetCore.Components.Web;

namespace Mekaniko_Secured.Models.Dto
{
    public class UnpaidInvoiceListDto
    {
        public int InvoiceId { get; set; }
        public string CarRego { get; set; }
        public string IssueName { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? isPaid { get; set; }
    }
}
