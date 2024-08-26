namespace Mekaniko_Secured.Models.Dto
{
    public class AddCarInvoiceDto
    {
        public int CarId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public string IssueName { get; set; }
        public string PaymentTerm { get; set; }
        public string Notes { get; set; }
        public decimal LaborPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsPaid { get; set; }
        public List<AddInvoiceItemDto> InvoiceItems { get; set; } = new List<AddInvoiceItemDto>();

    }
}
