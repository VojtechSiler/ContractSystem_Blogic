using System.ComponentModel.DataAnnotations;

namespace ContractSystem_Blogic.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(20, MinimumLength = 5)]
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}
