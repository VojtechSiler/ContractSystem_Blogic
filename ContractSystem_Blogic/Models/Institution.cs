using System.ComponentModel.DataAnnotations;

namespace ContractSystem_Blogic.Models
{
    public class Institution
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Contract>? Contracts { get; set; }
    }
}
