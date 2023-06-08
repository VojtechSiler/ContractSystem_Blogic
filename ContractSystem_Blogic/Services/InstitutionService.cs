using ContractSystem_Blogic.Data;
using ContractSystem_Blogic.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractSystem_Blogic.Services
{
    public class InstitutionService : IInstitutionService
    {
        private readonly DatabaseContext _context;

        public InstitutionService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateInstitution(Institution newInstitution)
        {
            _context.Institutions.Add(newInstitution);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInstitution(Guid id)
        {
            var institution = await _context.Institutions.FindAsync(id);

            if(institution == null)
            {
                throw new Exception("No institution with this ID.");
            }

            _context.Remove(institution);
            await _context.SaveChangesAsync();
        }

        public async Task<Institution> GetInstitutionById(Guid id)
        {
            var institution = await _context.Institutions.Include(i => i.Contracts).FirstOrDefaultAsync(i => i.Id == id);
            if(institution == null )
            {
                return null;
            }

            return institution;
        }

        public async Task<List<Institution>> GetInstitutions()
        {
            var institutions = await _context.Institutions.Include(i => i.Contracts).ToListAsync();
            return institutions;
        }

        public async Task UpdateInstitution(Institution updatedInstitution)
        {
            _context.Institutions.Update(updatedInstitution);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> InstitutionExists(string name, Guid institutionId)
        {
            var institution = await _context.Institutions.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name);
            if (institution != null)
            {
                if (institution.Id != institutionId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
