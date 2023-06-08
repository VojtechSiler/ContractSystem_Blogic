using ContractSystem_Blogic.Models;
using ContractSystem_Blogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractSystem_Blogic.Controllers
{
    [Authorize]
    public class InstitutionController : Controller
    {
        private readonly IInstitutionService _institutionService;

        public InstitutionController(IInstitutionService institutionService)
        {
            _institutionService = institutionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var institutions = await _institutionService.GetInstitutions();
            return View(institutions);
        }

        // CREATE
        [HttpGet]
        public IActionResult CreateInstitution()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInstitution(Institution newInstitution)
        {
            if (ModelState.IsValid)
            {
                if (await _institutionService.InstitutionExists(newInstitution.Name, newInstitution.Id))
                {
                    ViewData["CreateFlag"] = "Institution already exists.";
                    return View(newInstitution);
                }

                await _institutionService.CreateInstitution(newInstitution);
                return RedirectToAction("Index");
            }
            return View(newInstitution);
        }

        // UPDATE
        [HttpGet]
        public async Task<IActionResult> UpdateInstitution(Guid institutionId)
        {
            if (institutionId == Guid.Empty)
            {
                return BadRequest("Institution Id is null.");
            }

            var institution = await _institutionService.GetInstitutionById(institutionId);
            if(institution == null)
            {
                return NotFound("Institution not found.");
            }

            return View(institution);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInstitution(Institution updatedInstitution)
        {
            if (updatedInstitution.Id == Guid.Empty)
            {
                return BadRequest("Institution Id is null.");
            }

            if (ModelState.IsValid)
            {
                if (await _institutionService.InstitutionExists(updatedInstitution.Name, updatedInstitution.Id))
                {
                    ViewData["CreateFlag"] = "Institution already exists.";
                    return View(updatedInstitution);
                }

                await _institutionService.UpdateInstitution(updatedInstitution);
            }

            return RedirectToAction("Index");
        }

        // DELETE
        [HttpGet]
        public async Task<IActionResult> DeleteInstitution(Guid institutionId)
        {
            if (institutionId == Guid.Empty)
            {
                return BadRequest("Institution Id is null.");
            }

            var institution = await _institutionService.GetInstitutionById(institutionId);
            if (institution == null)
            {
                return NotFound("Institution not found.");
            }

            if (institution.Contracts.Count > 0)
            {
                return RedirectToAction("Index");
            }

            await _institutionService.DeleteInstitution(institutionId);
            return RedirectToAction("Index");
        }

    }
}
