using ContractSystem_Blogic.Models;
using ContractSystem_Blogic.Services;
using ContractSystem_Blogic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

namespace ContractSystem_Blogic.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IContractService _contractService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientController(IClientService clientService, IContractService contractService, IWebHostEnvironment webHostEnvironment)
        {
            _clientService = clientService;
            _contractService = contractService;
            _webHostEnvironment = webHostEnvironment;
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clients = await _clientService.GetClients();
            return View(clients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? searchString)
        {
            var clients = await _clientService.GetSearchedClients(searchString);
            return View(clients);
        }

        [HttpGet]
        public async Task<IActionResult> ClientDetail(Guid clientId)
        {
            if(clientId == Guid.Empty)
            {
                return BadRequest("Client Id is null.");
            }

            var clientContractViewModel = new ClientContractViewModel();

            var client = await _clientService.GetClientById(clientId);
            if(client == null)
            {
                return NotFound("Client not found.");
            }

            var clientsContracts = await _contractService.GetContractsForClient(clientId);

            clientContractViewModel.Client = client;
            clientContractViewModel.Contracts = clientsContracts;

            return View(clientContractViewModel);
        }

        // CREATE
        [HttpGet]
        public IActionResult CreateClient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClient(Client newClient, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                if(image != null)
                {
                    if (image.ContentType != "image/jpeg" && image.ContentType != "image/png")
                    {
                        ViewData["CreateFlag"] = "File is not PNG or JPG.";
                        return View(newClient);
                    }
                }

                if(!newClient.PhoneNumber.All(char.IsDigit))
                {
                    ViewData["CreateFlag"] = "Phone number must be numbers only.";
                    return View(newClient);
                }

                if (!ValidatePID(newClient.IdentificationNumber))
                {
                    ViewData["CreateFlag"] = "Identification number is not valid.";
                    return View(newClient);
                }

                if(await _clientService.ClientExists(newClient.IdentificationNumber, newClient.Id))
                {
                    ViewData["CreateFlag"] = "Client already exists.";
                    return View(newClient);
                }

                newClient = await SaveClientImage(newClient,image);
                await _clientService.CreateClient(newClient);
                return RedirectToAction("Index");
            }
            return View(newClient);
        }

        // UPDATE
        [HttpGet]
        public async Task<IActionResult> UpdateClient(Guid clientId)
        {
            if(clientId == Guid.Empty) 
            {
                return View();
            }

            var client = await _clientService.GetClientById(clientId);
            if(client == null)
            {
                return NotFound("Client not found.");
            }

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateClient(Client updatedClient, IFormFile? image)
        {
            if(updatedClient.Id == Guid.Empty)
            {
                return BadRequest("Client Id is null.");
            }

            if(ModelState.IsValid)
            {
                if (!ValidatePID(updatedClient.IdentificationNumber))
                {
                    ViewData["CreateFlag"] = "Identification number is not valid.";
                    return View(updatedClient);
                }

                if (await _clientService.ClientExists(updatedClient.IdentificationNumber, updatedClient.Id))
                {
                    ViewData["CreateFlag"] = "Client already exists.";
                    return View(updatedClient);
                }

                if (image != null)
                {
                    if (image.ContentType != "image/jpeg" && image.ContentType != "image/png")
                    {
                        ViewData["CreateFlag"] = "File is not PNG or JPG.";
                        return View(updatedClient);
                    }

                    updatedClient = await SaveClientImage(updatedClient, image);
                    await _clientService.UpdateClient(updatedClient, true);
                }
                else
                {
                    await _clientService.UpdateClient(updatedClient, false);
                }
            }
            return RedirectToAction("ClientDetail", "Client", new { clientId = updatedClient.Id });
        }

        // DELETE
        [HttpGet]
        public async Task<IActionResult> DeleteClient(Guid clientId)
        {
            if (clientId == Guid.Empty)
            {
                return BadRequest("Client Id is null.");
            }

            var client = await _clientService.GetClientById(clientId);
            if(client == null)
            {
                return NotFound("Client not found.");
            }

            if (client.Contracts.Count > 0)
            {
                return RedirectToAction("Index");
            }

            DeleteImage(client);

            await _clientService.DeleteClient(clientId);
            return RedirectToAction("Index");
        }


        // EXPORT TO CSV
        [HttpGet]
        public async Task<IActionResult> ExportClientsToCSV()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id, FirstName, LastName, Email, PhoneNumber, IdentificationNumber");
            var clients = await _clientService.GetClients();
            foreach (var client in clients)
            {
                builder.AppendLine($"{client.Id},{client.FirstName},{client.LastName},{client.Email},{client.PhoneNumber},{client.IdentificationNumber}");
            }
            return File(Encoding.GetEncoding("Windows-1250").GetBytes(builder.ToString()), "text/csv", "Clients.csv");
        }

        public async Task<Client> SaveClientImage(Client client, IFormFile? image)
        {
            if (image != null)
            {
                string new_id = Guid.NewGuid().ToString();
                var name = Path.Combine(_webHostEnvironment.WebRootPath + "/ProfilePictures", new_id + ".jpg");
                FileStream stream = new FileStream(name, FileMode.Create);
                await image.CopyToAsync(stream);
                stream.Dispose();
                client.ProfilePicture = "/ProfilePictures/" + new_id + ".jpg";
            }

            if (image == null)
            {
                client.ProfilePicture = "/ProfilePictures/no_picture.jpg";
            }
            return client;
        }

        public void DeleteImage(Client client)
        {
            if (client.ProfilePicture != "/ProfilePictures/no_picture.jpg")
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath + client.ProfilePicture));
                    fileInfo.Delete();
                }
                catch
                {

                }
            }
        }

        public bool ValidatePID(string PID)
        {
            if(PID.Length > 10 || PID.Length < 9) 
            {
                return false;
            }

            if(!PID.All(char.IsDigit))
            { 
                return false; 
            }

            if(PID.Length == 10)
            {
                int sum1 = 0;
                int sum2 = 0;

                for (int i = 0; i < PID.Length; i += 2)
                {
                    int digit = Int32.Parse(PID.Substring(i, 1));
                    sum1 += digit;
                }

                for (int i = 1; i < PID.Length; i += 2)
                {
                    int digit = Int32.Parse(PID.Substring(i, 1));
                    sum2 += digit;
                }

                int difference = Math.Abs(sum1 - sum2);

                if (difference != 0 && difference != 11 && difference != 22)
                {
                    return false;
                }
            }
            
            if ( Int32.Parse(PID.Substring(2, 2)) > 82 || Int32.Parse(PID.Substring(4, 2)) > 31)
            {
                return false;
            }

            return true;
        }
    }
}
