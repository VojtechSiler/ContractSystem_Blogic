using ContractSystem_Blogic.Data;
using ContractSystem_Blogic.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Runtime.CompilerServices;
using ContractSystem_Blogic.Services;
using ContractSystem_Blogic.DTOs;

namespace ContractSystem_Blogic.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IOptions<AppSettings> _options;

        public AccountController(IOptions<AppSettings> options, IAccountService accountService)
        {
            _options = options;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Client");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if(ModelState.IsValid)
            {
                var user = await _accountService.AuthenticateUser(loginDTO);

                if (user == null)
                {
                    ViewData["LoginFlag"] = "Username or password is invalid.";
                    return View();
                }

                var token = GenerateJwtToken(user);

                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                });

                return RedirectToAction("Index", "Client");
            }
            return View(loginDTO);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if(ModelState.IsValid)
            {
                if(registerDTO.Password != registerDTO.PasswordRepeat)
                {
                    ViewData["RegisterFlag"] = "Passwords must match.";
                    return View();
                }

                if(await _accountService.UsernameExists(registerDTO.Username))
                {
                    ViewData["RegisterFlag"] = "Username is already in use.";
                    return View();
                }

                if(await _accountService.EmailExists(registerDTO.Email))
                {
                    ViewData["RegisterFlag"] = "Email is already in use.";
                    return View();
                }

                await _accountService.RegisterUser(registerDTO);
                return RedirectToAction("Login");
            }
            return View(registerDTO);
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_options.Value.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
