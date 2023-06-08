using ContractSystem_Blogic.DTOs;
using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.Services
{
    public interface IAccountService
    {
        public Task RegisterUser(RegisterDTO registerDTO);
        public Task<User> AuthenticateUser(LoginDTO loginDTO);
        public Task<bool> UsernameExists(string  username);
        public Task<bool> EmailExists(string email);
    }
}
