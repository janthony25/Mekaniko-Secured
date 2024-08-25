using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mekaniko_Secured.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 5)]
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string Role { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
