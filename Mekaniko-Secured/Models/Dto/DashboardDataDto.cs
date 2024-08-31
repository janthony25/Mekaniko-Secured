namespace Mekaniko_Secured.Models.Dto
{
    public class DashboardDataDto
    {
        public decimal TotalInvoiceAmount { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalCars { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public List<UnpaidInvoiceListDto> UnpaidInvoices { get; set; }
    }
}
