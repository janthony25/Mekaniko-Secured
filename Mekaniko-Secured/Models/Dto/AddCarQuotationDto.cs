namespace Mekaniko_Secured.Models.Dto
{
    public class AddCarQuotationDto
    {
        public int CarId { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public required string IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<AddQuotationItemDto> QuotationItems { get; set; } = new List<AddQuotationItemDto>();
    }
}
