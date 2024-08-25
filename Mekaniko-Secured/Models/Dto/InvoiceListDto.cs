namespace Mekaniko_Secured.Models.Dto
{
    public class InvoiceListDto
    {
        public int InvoiceId { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? IsPaid { get; set; }

        // CUSTOMER
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // CAR
        public int CarId { get; set; }
        public string CarRego { get; set; }
    }
}
