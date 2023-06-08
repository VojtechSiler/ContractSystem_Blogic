using ContractSystem_Blogic.Data;
using ContractSystem_Blogic.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ContractSystem_Blogic.Services
{
    public class ConsultantService : IConsultantService
    {
        private readonly DatabaseContext _context;

        public ConsultantService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateConsultant(Consultant newConsultant)
        {
            _context.Consultants.Add(newConsultant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConsultant(Guid id)
        {
            var consultant = await _context.Consultants.FindAsync(id);
            if (consultant == null)
            {
                throw new Exception("No consultant with this ID.");
            }

            _context.Consultants.Remove(consultant);
            await _context.SaveChangesAsync();
        }

        public async Task<Consultant> GetConsultantById(Guid id)
        {
            var consultant = await _context.Consultants.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.Id == id);
            if (consultant == null)
            {
                return null;
            }

            return consultant;
        }

        public async Task<List<Consultant>> GetConsultants()
        {
            var consultants = await _context.Consultants.ToListAsync();
            return consultants;
        }

        public async Task UpdateConsultant(Consultant updatedConsultant, bool newPicture)
        {
            var originalConsultant = await _context.Consultants.AsNoTracking().FirstOrDefaultAsync(c => c.Id == updatedConsultant.Id);
            if(originalConsultant == null)
            {
                throw new Exception("No consultant with this ID.");
            }

            if (!newPicture)
            {
                updatedConsultant.ProfilePicture = originalConsultant.ProfilePicture;
            }

            _context.Consultants.Update(updatedConsultant);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Consultant>> GetSearchedConsultants(string searchString)
        {
            var searchedConsultants = new List<Consultant>();

            if (string.IsNullOrEmpty(searchString))
            {
                searchedConsultants = await GetConsultants();
            }
            else
            {
                searchedConsultants = await _context.Consultants.Where(c => c.FirstName.Contains(searchString) || c.LastName.Contains(searchString) || c.IdentificationNumber.Contains(searchString)).ToListAsync();
            }

            return searchedConsultants;
        }

        public async Task<bool> ConsultantExists(string identificationNumber, Guid consultantId)
        {
            var consultant = await _context.Consultants.AsNoTracking().FirstOrDefaultAsync(c => c.IdentificationNumber == identificationNumber);
            if (consultant != null)
            {
                if (consultant.Id != consultantId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
