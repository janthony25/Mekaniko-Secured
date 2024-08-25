namespace Mekaniko_Secured.Models.Dto
{
    public class AddCarDto
    {
        public required string CarRego { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public int CustomerId { get; set; }
        public int MakeId { get; set; }
    }
}
