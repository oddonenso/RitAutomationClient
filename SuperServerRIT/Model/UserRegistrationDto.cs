using System.ComponentModel.DataAnnotations;

namespace SuperServerRIT.Model
{
    public class UserRegistrationDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }   = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
