using Azure;
using ContractSystem_Blogic.Data;
using ContractSystem_Blogic.DTOs;
using ContractSystem_Blogic.Models;

namespace ContractSystem_Blogic.Services
{
    public class AccountService : IAccountService
    {
        private readonly DatabaseContext _context;

        public AccountService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateUser(LoginDTO loginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDTO.Username);

            if(user == null)
            {
                return null;
            }

            bool passwordVerify = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash);

            if(passwordVerify)
            {
                return user;
            }
            else
            {
                return null;
            }

        }

        public async Task<bool> EmailExists(string email)
        {
            if (_context.Users.Any(u => u.Email == email))
                return true;
            else
                return false;
        }

        public async Task<bool> UsernameExists(string username)
        {
            if (_context.Users.Any(u => u.Username == username))
                return true;
            else
                return false;
        }

        public async Task RegisterUser(RegisterDTO registerDTO)
        {
            var newUser = new User();
            newUser.Username = registerDTO.Username;
            newUser.Email = registerDTO.Email;
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }
    }
}
