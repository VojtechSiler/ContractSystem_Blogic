using ContractSystem_Blogic.Data;
using ContractSystem_Blogic.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractSystem_Blogic.Services
{
    public class ClientService : IClientService
    {
        private readonly DatabaseContext _context;
        public ClientService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateClient(Client newClient)
        {
            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClient(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);

            if(client == null)
            {
                throw new Exception("No client with this ID.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        public async Task<Client> GetClientById(Guid id)
        {
            var client = await _context.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.Id == id);
            if(client == null) 
            {
                return null;
            }

            return client;
        }

        public async Task<List<Client>> GetClients()
        {
            var clients = await _context.Clients.ToListAsync();
            return clients;
        }

        public async Task UpdateClient(Client updatedClient, bool newPicture)
        {
            var originalClient = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == updatedClient.Id);
            if(originalClient == null)
            {
                throw new Exception("No client with this ID.");
            }

            if (!newPicture)
            {
                updatedClient.ProfilePicture = originalClient.ProfilePicture;
            }

            _context.Clients.Update(updatedClient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Client>> GetSearchedClients(string searchString)
        {
            var searchedClients = new List<Client>();

            if(string.IsNullOrEmpty(searchString))
            {
                searchedClients = await GetClients();
            }
            else
            {
                searchedClients = await _context.Clients.Where(c => c.FirstName.Contains(searchString) || c.LastName.Contains(searchString) || c.IdentificationNumber.Contains(searchString)).ToListAsync();
            }

            return searchedClients;
        }

        public async Task<bool> ClientExists(string identificationNumber, Guid clientId)
        {
            var client = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.IdentificationNumber == identificationNumber);
            if (client != null)
            {
                if (client.Id != clientId)
                    return true;
                else
                    return false;
            } 
            else
                return false;
        }
    }
}
