using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.ViewModels
{
    public class ConsultantContractViewModel
    {
        public Consultant Consultant { get; set; }
        public List<Contract> Contracts { get; set; }
    }
}
