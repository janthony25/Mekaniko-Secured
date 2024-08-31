namespace Mekaniko_Secured.Models.Dto
{
    public class InvoiceListDto
    {
        public int InvoiceId { get; set; }
        public string IssueName { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsEmailSent { get; set; } // will mark the email as sent

        // CUSTOMER
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // CAR
        public int CarId { get; set; }
        public string CarRego { get; set; }
    }
}
