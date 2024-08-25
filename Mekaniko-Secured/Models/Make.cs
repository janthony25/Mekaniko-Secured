using System.ComponentModel.DataAnnotations;

namespace Mekaniko_Secured.Models
{
    public class Make
    {
        [Key]
        public int MakeId { get; set; }
        public required string MakeName { get; set; }

        // M-to-M Car-Make
        public List<CarMake> CarMake { get; set; }
    }
}
