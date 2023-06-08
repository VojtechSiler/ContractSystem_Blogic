using ContractSystem_Blogic.DTOs;
using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.ViewModels
{
    public class ContractCUViewModel
    {
        public List<Institution> Institutions { get; set; }
        public List<Client> Clients { get; set; }
        public List<Consultant> Consultants { get; set; }
        public ContractDTO Contract { get; set; }
    }
}
