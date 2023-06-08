using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.Services
{
    public interface IClientService
    {
        public Task<List<Client>> GetClients();
        public Task<Client> GetClientById(Guid id);
        public Task CreateClient(Client newClient);
        public Task UpdateClient(Client updatedClient, bool newPicture);
        public Task DeleteClient(Guid id);
        public Task<List<Client>> GetSearchedClients(string searchString);
        public Task<bool> ClientExists(string identificationNumber, Guid clientId);
    }
}
