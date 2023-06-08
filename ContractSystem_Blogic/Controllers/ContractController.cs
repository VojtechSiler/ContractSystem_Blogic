using ContractSystem_Blogic.DTOs;
using ContractSystem_Blogic.Models;
using ContractSystem_Blogic.Services;
using ContractSystem_Blogic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Text;

namespace ContractSystem_Blogic.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IClientService _clientService;
        private readonly IConsultantService _consultantService;
        private readonly IInstitutionService _institutionService;

        public ContractController(IContractService contractService, IClientService clientService, IConsultantService consultantService, IInstitutionService institutionService)
        {
            _contractService = contractService;
            _clientService = clientService;
            _consultantService = consultantService;
            _institutionService = institutionService;
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var contracts = await _contractService.GetContracts();
            return View(contracts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? searchString)
        {
            var contracts = await _contractService.GetSearchedContracts(searchString);
            return View(contracts);
        }

        [HttpGet]
        public async Task<IActionResult> ContractDetail(Guid contractId)
        {
            if(contractId == Guid.Empty)
            {
                return BadRequest("Contract Id is null.");
            }

            var contract = await _contractService.GetContractById(contractId);
            if(contract == null)
            {
                return NotFound("Contract not found.");
            }

            return View(contract);
        }

        // CREATE
        [HttpGet]
        public async Task<IActionResult> CreateContract()
        {
            var contractViewModel = new ContractCUViewModel();

            contractViewModel.Clients = await _clientService.GetClients();
            contractViewModel.Consultants = await _consultantService.GetConsultants();
            contractViewModel.Institutions = await _institutionService.GetInstitutions();

            return View(contractViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContract(ContractDTO contract)
        {
            var contractViewModel = new ContractCUViewModel();

            contractViewModel.Clients = await _clientService.GetClients();
            contractViewModel.Consultants = await _consultantService.GetConsultants();
            contractViewModel.Institutions = await _institutionService.GetInstitutions();

            if (ModelState.IsValid)
            {
                if(await _contractService.ContractExists(contract.RegistrationNumber, contract.Id))
                {
                    ViewData["CreateFlag"] = "Contract with this registration number already exists.";
                    contractViewModel.Contract = contract;
                    return View(contractViewModel);
                }

                await _contractService.CreateContract(contract);
                return RedirectToAction("Index");
            }
            contractViewModel.Contract = contract;
            return View(contractViewModel);
        }

        // UPDATE
        [HttpGet]
        public async Task<ActionResult> UpdateContract(Guid contractId)
        {
            if(contractId == Guid.Empty)
            {
                return BadRequest("Contract Id is null.");
            }

            var contractCUViewModel = await _contractService.ContractToDTO(contractId);
            if(contractCUViewModel == null)
            {
                return NotFound("Contract not found.");
            }
            
            return View(contractCUViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateContract(ContractDTO contract)
        {
            if (await _contractService.ContractExists(contract.RegistrationNumber, contract.Id))
            {
                var contractCUViewModel = await _contractService.ContractToDTO(contract.Id);
                if (contractCUViewModel == null)
                {
                    return NotFound("Contract not found.");
                }
                contractCUViewModel.Contract = contract;
                ViewData["CreateFlag"] = "Contract with this registration number already exists.";
                return View(contractCUViewModel);
            }

            if (ModelState.IsValid)
            {
                await _contractService.UpdateContract(contract);
            }
            return RedirectToAction("Index");
        }

        // DELETE
        [HttpGet]
        public async Task<ActionResult> DeleteContract(Guid contractId)
        {
            if(contractId == Guid.Empty)
            {
                return BadRequest("Contract Id is null.");
            }

            await _contractService.DeleteContract(contractId);
            return RedirectToAction("Index");
        }

        // EXPORT TO CSV
        public async Task<IActionResult> ExportContractsToCSV()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id, RegistrationNumber, ClientId, ContractManagerId, ConsultantIds, ClosureDate, StartDate, EndDate");
            var contracts = await _contractService.GetContracts();
            foreach (var contract in contracts)
            {
                string consultantIds = string.Join(" ", contract.Consultants.Select(consultant => consultant.Id.ToString()));
                builder.AppendLine($"{contract.Id},{contract.RegistrationNumber}, {contract.ClientId}, {contract.ContractManagerId}, {consultantIds},{contract.ClosureDate.Day+"."+contract.ClosureDate.Month + "." + contract.ClosureDate.Year},{contract.StartDate.Day + "." + contract.StartDate.Month + "." + contract.StartDate.Year},{contract.EndDate.Day + "." + contract.EndDate.Month + "." + contract.EndDate.Year}");
            }
            return File(Encoding.GetEncoding("Windows-1250").GetBytes(builder.ToString()), "text/csv", "Consultants.csv");
        }
    }
}
