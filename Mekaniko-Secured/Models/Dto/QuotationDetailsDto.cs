namespace Mekaniko_Secured.Models.Dto
{
    public class QuotationDetailsDto
    {
        public int QuotationId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public int CarId { get; set; }
        public string CarRego { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public string MakeName { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public string IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<QuotationItemDetailsDto> QuotationItems { get; set; }
    }
}
