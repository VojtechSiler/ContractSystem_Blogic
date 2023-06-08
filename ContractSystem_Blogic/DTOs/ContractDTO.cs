using ContractSystem_Blogic.Models;
using System.ComponentModel.DataAnnotations;

namespace ContractSystem_Blogic.DTOs
{
    public class ContractDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string RegistrationNumber { get; set; }

        [Required]
        public Guid InstitutionId { get; set; }

        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public Guid ContractManagerId { get; set; }

        public List<Guid> ConsultantIds { get; set; }

        [Required]
        public DateTime ClosureDate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
