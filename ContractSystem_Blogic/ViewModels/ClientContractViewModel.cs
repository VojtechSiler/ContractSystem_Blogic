using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.ViewModels
{
    public class ClientContractViewModel
    {
        public Client Client { get; set; }
        public List<Contract> Contracts { get; set; }
    }
}
