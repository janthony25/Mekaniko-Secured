namespace Mekaniko_Secured.Models.Dto
{
    public class QuotationListDto
    {
        public int QuotationId { get; set; }
        public string CustomerName { get; set; }
        public int CarId { get; set; }
        public string CarRego { get; set; }
        public string IssueName { get; set; }
        public DateTime? DateAdded { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? IsEmailSent { get; set; }   
    }
}
