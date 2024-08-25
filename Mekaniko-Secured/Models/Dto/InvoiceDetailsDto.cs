namespace Mekaniko_Secured.Models.Dto
{
    public class InvoiceDetailsDto
    {
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerNumber { get; set; }
        public int CarId { get; set; }
        public string CarRego { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DueDate { get; set; }
        public string IssueName { get; set; }
        public string PaymentTerm { get; set; }
        public string Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool? IsPaid { get; set; }
        public List<InvoiceItemDto> InvoiceItems { get; set; }
    }
}
