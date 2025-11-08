using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User"; 
    }
}
