namespace Mekaniko_Secured.Models.Dto
{
    public class CarQuotationSummaryDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public int CarId { get; set; }
        public string CarRego { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public string MakeName { get; set; }
        public int QuotationId { get; set; }
        public DateTime? DateAdded { get; set; }
        public string IssueName { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
