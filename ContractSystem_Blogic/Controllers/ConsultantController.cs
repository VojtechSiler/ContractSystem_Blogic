using ContractSystem_Blogic.Models;
using ContractSystem_Blogic.Services;
using ContractSystem_Blogic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;
using System.Text;

namespace ContractSystem_Blogic.Controllers
{
    [Authorize]
    public class ConsultantController : Controller
    {
        private readonly IConsultantService _consultantService;
        private readonly IContractService _contractService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConsultantController(IConsultantService consultantService, IContractService contractService, IWebHostEnvironment webHostEnvironment)
        {
            _consultantService = consultantService;
            _contractService = contractService;
            _webHostEnvironment = webHostEnvironment;
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var consultants = await _consultantService.GetConsultants();
            return View(consultants);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? searchString)
        {
            var consultants = await _consultantService.GetSearchedConsultants(searchString);
            return View(consultants);
        }

        [HttpGet]
        public async Task<IActionResult> ConsultantDetail(Guid consultantId)
        {
            if(consultantId == Guid.Empty)
            {
                return BadRequest("Consultant Id is null.");
            }

            var consultantContractViewModel = new ConsultantContractViewModel();

            var consultant = await _consultantService.GetConsultantById(consultantId);
            if(consultant == null)
            {
                return NotFound("Consultant not found.");
            }

            var consultantsContracts = await _contractService.GetContractsForConsultant(consultantId);

            consultantContractViewModel.Consultant = consultant;
            consultantContractViewModel.Contracts = consultantsContracts;

            return View(consultantContractViewModel);
        }

        // CREATE
        [HttpGet]
        public IActionResult CreateConsultant()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConsultant(Consultant newConsultant, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    if (image.ContentType != "image/jpeg" && image.ContentType != "image/png")
                    {
                        ViewData["CreateFlag"] = "File is not PNG or JPG.";
                        return View(newConsultant);
                    }
                }

                if (!ValidatePID(newConsultant.IdentificationNumber))
                {
                    ViewData["CreateFlag"] = "Identification number is not valid.";
                    return View(newConsultant);
                }

                if (!newConsultant.PhoneNumber.All(char.IsDigit))
                {
                    ViewData["CreateFlag"] = "Phone number must be numbers only.";
                    return View(newConsultant);
                }

                if (await _consultantService.ConsultantExists(newConsultant.IdentificationNumber, newConsultant.Id))
                {
                    ViewData["CreateFlag"] = "Client already exists.";
                    return View(newConsultant);
                }

                newConsultant = await SaveConsultantImage(newConsultant, image);
                await _consultantService.CreateConsultant(newConsultant);
                return RedirectToAction("Index");
            }
            return View(newConsultant);
        }

        // UPDATE
        [HttpGet]
        public async Task<IActionResult> UpdateConsultant(Guid consultantId)
        {
            if (consultantId == Guid.Empty)
            {
                return BadRequest("Consultant Id is null.");
            }

            var consultant = await _consultantService.GetConsultantById(consultantId);
            if(consultant == null)
            {
                return NotFound("Consultant not found.");
            }

            return View(consultant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateConsultant(Consultant updatedConsultant, IFormFile? image)
        {
            if (updatedConsultant.Id == Guid.Empty)
            {
                return BadRequest("Consultant Id is null.");
            }

            if (ModelState.IsValid)
            {
                if (!ValidatePID(updatedConsultant.IdentificationNumber))
                {
                    ViewData["CreateFlag"] = "Identification number is not valid.";
                    return View(updatedConsultant);
                }

                if (await _consultantService.ConsultantExists(updatedConsultant.IdentificationNumber, updatedConsultant.Id))
                {
                    ViewData["CreateFlag"] = "Client already exists.";
                    return View(updatedConsultant);
                }

                if (image != null)
                {
                    if (image.ContentType != "image/jpeg" && image.ContentType != "image/png")
                    {
                        ViewData["CreateFlag"] = "File is not PNG or JPG.";
                        return View(updatedConsultant);
                    }

                    updatedConsultant = await SaveConsultantImage(updatedConsultant, image);
                    await _consultantService.UpdateConsultant(updatedConsultant, true);
                }
                else
                {
                    await _consultantService.UpdateConsultant(updatedConsultant, false);
                }
            }
            return RedirectToAction("ConsultantDetail", "Consultant", new { consultantId = updatedConsultant.Id });
        }

        // DELETE
        [HttpGet]
        public async Task<IActionResult> DeleteConsultant(Guid consultantId)
        {
            if(consultantId == Guid.Empty)
            {
                return BadRequest("Consultant Id is null");
            }

            var consultant = await _consultantService.GetConsultantById(consultantId);
            if(consultant == null)
            {
                return NotFound("Consultant not found.");
            }

            if(consultant.Contracts.Count > 0)
            {
                return RedirectToAction("Index");
            }

            DeleteImage(consultant);

            await _consultantService.DeleteConsultant(consultantId);
            return RedirectToAction("Index");
        }

        // EXPORT TO CSV
        [HttpGet]
        public async Task<IActionResult> ExportConsultantsToCSV()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id, FirstName, LastName, Email, PhoneNumber, IdentificationNumber");
            var consultants = await _consultantService.GetConsultants();
            foreach (var consultant in consultants)
            {
                builder.AppendLine($"{consultant.Id},{consultant.FirstName},{consultant.LastName},{consultant.Email},{consultant.PhoneNumber},{consultant.IdentificationNumber}");
            }
            return File(Encoding.GetEncoding("Windows-1250").GetBytes(builder.ToString()), "text/csv", "Consultants.csv");
        }

        public async Task<Consultant> SaveConsultantImage(Consultant consultant, IFormFile? image)
        {
            if (image != null)
            {
                string new_id = Guid.NewGuid().ToString();
                var name = Path.Combine(_webHostEnvironment.WebRootPath + "/ProfilePictures", new_id + ".jpg");
                FileStream stream = new FileStream(name, FileMode.Create);
                await image.CopyToAsync(stream);
                stream.Dispose();
                consultant.ProfilePicture = "/ProfilePictures/" + new_id + ".jpg";
            }

            if (image == null)
            {
                consultant.ProfilePicture = "/ProfilePictures/no_picture.jpg";
            }
            return consultant;
        }

        public void DeleteImage(Consultant consultant)
        {
            if (consultant.ProfilePicture != "/ProfilePictures/no_picture.png")
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath + consultant.ProfilePicture));
                    fileInfo.Delete();
                }
                catch
                {

                }
            }
        }

        public bool ValidatePID(string PID)
        {
            if (PID.Length > 10 || PID.Length < 9)
            {
                return false;
            }

            if (!PID.All(char.IsDigit))
            {
                return false;
            }

            if (PID.Length == 10)
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

            if (Int32.Parse(PID.Substring(2, 2)) > 82 || Int32.Parse(PID.Substring(4, 2)) > 31)
            {
                return false;
            }

            return true;
        }

    }
}
