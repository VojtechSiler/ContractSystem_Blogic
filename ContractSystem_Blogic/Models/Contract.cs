using System.ComponentModel.DataAnnotations;

namespace ContractSystem_Blogic.Models
{
    public class Contract
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "Registration number must be 6 digits long.")]
        public string RegistrationNumber { get; set; }

        [Required]
        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; }

        [Required]
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        [Required]
        public Guid ContractManagerId { get; set; }
        public Consultant ContractManager { get; set; }

        public ICollection<Consultant> Consultants { get; set; }

        [Required]
        public DateTime ClosureDate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }


    }
}
