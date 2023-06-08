using ContractSystem_Blogic.Data;
using ContractSystem_Blogic.DTOs;
using ContractSystem_Blogic.Models;
using ContractSystem_Blogic.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ContractSystem_Blogic.Services
{
    public class ContractService : IContractService
    {
        private readonly DatabaseContext _context;
        private readonly IClientService _clientService;
        private readonly IConsultantService _consultantService;
        private readonly IInstitutionService _institutionService;

        public ContractService(DatabaseContext context, IClientService clientService, IConsultantService consultantService, IInstitutionService institutionService)
        {
            _context = context;
            _clientService = clientService;
            _consultantService = consultantService;
            _institutionService = institutionService;
        }

        public async Task CreateContract(ContractDTO newContractDTO)
        {
            var contract = new Contract();

            contract.RegistrationNumber = newContractDTO.RegistrationNumber;
            contract.ClientId = newContractDTO.ClientId;
            contract.ContractManagerId = newContractDTO.ContractManagerId;
            contract.InstitutionId = newContractDTO.InstitutionId;
            contract.ClosureDate = newContractDTO.ClosureDate;
            contract.StartDate = newContractDTO.StartDate;
            contract.EndDate = newContractDTO.EndDate;

            var consultants = await _context.Consultants.Where(c => newContractDTO.ConsultantIds.Contains(c.Id)).ToListAsync();
            contract.Consultants = consultants;

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContract(Guid id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if(contract == null)
            {
                throw new Exception("No contract with this ID.");
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<Contract> GetContractById(Guid id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ContractManager)
                .Include(c => c.Consultants)
                .Include(c => c.Institution)
                .FirstOrDefaultAsync(c => c.Id == id);

            if(contract == null)
            {
                return null;
            }

            return contract;
        }

        public async Task<List<Contract>> GetContracts()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ContractManager)
                .Include(c => c.Institution)
                .Include(c => c.Consultants)
                .ToListAsync();

            return contracts;
        }

        public async Task<List<Contract>> GetContractsForClient(Guid clientId)
        {
            if(clientId == null)
            {
                return null;
            }

            var clientCheck = await _context.Clients.FindAsync(clientId);

            if(clientCheck == null)
            {
                return null;
            }

            var contracts = await _context.Contracts
                .Include(c => c.Institution)
                .Include(c => c.ContractManager)
                .Where(c =>  c.ClientId == clientId)
                .ToListAsync();

            return contracts;
        }

        public async Task<List<Contract>> GetContractsForConsultant(Guid consultantId)
        {
            var consultant = await _context.Consultants.FindAsync(consultantId);

            if(consultant == null)
            {
                return null;
            }

            var consultantsContracts = await _context.Contracts
                .Include(c => c.Consultants)
                .Include(c => c.Institution)
                .Include(c => c.Client)
                .Include(c => c.ContractManager)
                .Where(c => c.Consultants
                .Any(cc => cc.Id == consultantId))
                .ToListAsync();

            return consultantsContracts;
        }

        public async Task<List<Contract>> GetContractsForInstitution(Guid institutionId)
        {
            var institutionCheck = await _context.Institutions.FindAsync(institutionId);

            if (institutionCheck == null)
            {
                return null;
            }

            var contracts = await _context.Contracts.Where(c => c.InstitutionId == institutionId).ToListAsync();

            return contracts;
        }

        public async Task UpdateContract(ContractDTO updatedContractDTO)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ContractManager)
                .Include(c => c.Consultants)
                .Include(c => c.Institution)
                .FirstOrDefaultAsync(c => c.Id == updatedContractDTO.Id);

            if(contract == null)
            {
                throw new Exception("No contract with this ID.");
            }

            contract.ClosureDate = updatedContractDTO.ClosureDate;
            contract.StartDate = updatedContractDTO.StartDate;
            contract.EndDate = updatedContractDTO.EndDate;
            contract.ClientId = updatedContractDTO.ClientId;
            contract.InstitutionId = updatedContractDTO.InstitutionId;
            contract.ContractManagerId = updatedContractDTO.ContractManagerId;
            contract.RegistrationNumber = updatedContractDTO.RegistrationNumber;

            var consultants = await _context.Consultants.Where(c => updatedContractDTO.ConsultantIds.Contains(c.Id)).ToListAsync();
            contract.Consultants = consultants;

            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Contract>> GetSearchedContracts(string searchString)
        {
            var searchedContracts = new List<Contract>();

            if (string.IsNullOrEmpty(searchString))
            {
                searchedContracts = await GetContracts();
            }
            else
            {
                searchedContracts = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ContractManager)
                .Include(c => c.Institution)
                .Where(c => c.RegistrationNumber.Contains(searchString) || c.ContractManager.FirstName.Contains(searchString) || c.ContractManager.LastName.Contains(searchString) || c.Client.FirstName.Contains(searchString) || c.Client.LastName.Contains(searchString)).ToListAsync();
            }

            return searchedContracts;
        }

        public async Task<ContractCUViewModel> ContractToDTO(Guid contractId)
        {
            var contractDTO = new ContractDTO();
            var contract = await GetContractById(contractId);
            if (contract == null)
            {
                return null;
            }

            contractDTO.RegistrationNumber = contract.RegistrationNumber;
            contractDTO.ClosureDate = contract.ClosureDate;
            contractDTO.EndDate = contract.EndDate;
            contractDTO.StartDate = contract.StartDate;
            contractDTO.ContractManagerId = contract.ContractManagerId;
            contractDTO.ClientId = contract.ClientId;
            contractDTO.InstitutionId = contract.InstitutionId;
            contractDTO.Id = contract.Id;

            contractDTO.ConsultantIds = new List<Guid>();
            foreach (var consultant in contract.Consultants)
            {
                contractDTO.ConsultantIds.Add(consultant.Id);
            }

            var contractCUViewModel = new ContractCUViewModel();
            contractCUViewModel.Contract = contractDTO;
            contractCUViewModel.Clients = await _clientService.GetClients();
            contractCUViewModel.Consultants = await _consultantService.GetConsultants();
            contractCUViewModel.Institutions = await _institutionService.GetInstitutions();

            return contractCUViewModel;
        }

        public async Task<bool> ContractExists(string registrationNumber, Guid contractId)
        {
            var contract = await _context.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.RegistrationNumber == registrationNumber);
            if (contract != null)
            {
                if (contract.Id != contractId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
