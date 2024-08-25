namespace Mekaniko_Secured.Models.Dto
{
    public class CustomerSummaryDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerEmail { get; set; }
    }
}
