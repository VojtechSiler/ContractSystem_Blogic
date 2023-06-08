using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.Services
{
    public interface IInstitutionService
    {
        public Task<List<Institution>> GetInstitutions();
        public Task<Institution> GetInstitutionById(Guid id);
        public Task CreateInstitution(Institution newInstitution);
        public Task UpdateInstitution(Institution updatedInstitution);
        public Task DeleteInstitution(Guid id);
        public Task<bool> InstitutionExists(string name, Guid institutionId);
    }
}
