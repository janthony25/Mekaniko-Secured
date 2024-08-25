namespace Mekaniko_Secured.Models
{
    public class CarMake
    {
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int MakeId { get; set; }
        public Make Make { get; set; }
    }
}
