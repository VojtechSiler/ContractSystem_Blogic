using System.ComponentModel.DataAnnotations;

namespace ContractSystem_Blogic.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
