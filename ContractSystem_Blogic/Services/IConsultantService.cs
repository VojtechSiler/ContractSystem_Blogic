using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.Services
{
    public interface IConsultantService
    {
        public Task<List<Consultant>> GetConsultants();
        public Task<Consultant> GetConsultantById(Guid id);
        public Task CreateConsultant(Consultant newConsultant);
        public Task UpdateConsultant(Consultant updatedConsultant, bool newPicture);
        public Task DeleteConsultant(Guid id);
        public Task<List<Consultant>> GetSearchedConsultants(string searchString);
        public Task<bool> ConsultantExists(string identificationNumber, Guid consultantId);
    }
}
