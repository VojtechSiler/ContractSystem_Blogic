using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Security.Cryptography;

namespace ContractSystem_Blogic.Models
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Phone number must be 9 digits long.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 9, ErrorMessage = "Identification number must be 9 or 10 characters long.")]
        [DisplayName("Identification Number")]
        public string IdentificationNumber { get; set; }

        public string? ProfilePicture { get; set; }

        public ICollection<Contract>? Contracts { get; set; }

        [NotMapped]
        public int? Age { get { return CalculateAge(); } }

        public int CalculateAge()
        {
            if(this.IdentificationNumber != null && this.IdentificationNumber.All(char.IsDigit))
            {
                string dateOfBirthString = this.IdentificationNumber.Substring(0, 6);

                int thirdAndFourthNumbers = Int32.Parse(dateOfBirthString.Substring(2, 2));

                if (thirdAndFourthNumbers > 70)
                {
                    thirdAndFourthNumbers -= 70;
                }
                else if (thirdAndFourthNumbers > 50)
                {
                    thirdAndFourthNumbers -= 50;
                }

                dateOfBirthString = dateOfBirthString.Substring(0, 2) + thirdAndFourthNumbers.ToString("D2") + dateOfBirthString.Substring(4);

                try
                {
                    DateTime dateOfBirth = DateTime.ParseExact(dateOfBirthString, "yyMMdd", CultureInfo.InvariantCulture);
                    int age = DateTime.Now.Year - dateOfBirth.Year;
                    return age;
                }
                catch 
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
