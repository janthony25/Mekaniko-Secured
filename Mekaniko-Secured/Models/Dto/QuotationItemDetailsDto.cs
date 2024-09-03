namespace Mekaniko_Secured.Models.Dto
{
    public class QuotationItemDetailsDto
    {
        public int QuotationItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal? ItemTotal { get; set; }
    }
}
