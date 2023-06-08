using ContractSystem_Blogic.DTOs;
using ContractSystem_Blogic.Models;
using ContractSystem_Blogic.ViewModels;

namespace ContractSystem_Blogic.Services
{
    public interface IContractService
    {
        public Task<List<Contract>> GetContracts();
        public Task<Contract> GetContractById(Guid id);
        public Task<List<Contract>> GetContractsForClient(Guid clientId);
        public Task<List<Contract>> GetContractsForConsultant(Guid consultantId);
        public Task<List<Contract>> GetContractsForInstitution(Guid institutionId);
        public Task CreateContract(ContractDTO newContractDTO);
        public Task UpdateContract(ContractDTO updatedContractDTO);
        public Task DeleteContract(Guid id);
        Task<List<Contract>> GetSearchedContracts(string searchString);
        public Task<bool> ContractExists(string registrationNumber, Guid contractId);
        Task<ContractCUViewModel> ContractToDTO(Guid contractId);
    }
}
