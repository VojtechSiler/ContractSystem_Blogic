using System.ComponentModel.DataAnnotations;

namespace ContractSystem_Blogic.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Must be between 5 and 20 characters.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 9)]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Confirm Password field is required.")]
        public string PasswordRepeat { get; set; }
    }
}
