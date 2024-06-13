using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Data;
using OnlineSchool.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlineSchool.Services;
using OnlineSchool.ViewModels;

namespace OnlineSchool.Controllers
{
    public class AuthController : Controller
    {
        private readonly OnlineSchoolDbContext _context;

        public AuthController(OnlineSchoolDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "School");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginUser)
        {
            if (ModelState.IsValid)
            {
                var loginPass = loginUser.Password;
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginUser.Email);
                if (user != null && PasswordHasher.VerifyPassword(user.Password, loginPass))
                {
                    if (user != null)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Fullname),
                    new Claim(ClaimTypes.Email, user.Email),
                };
                        if (user.RoleId == 5)
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "None"));
                        }
                        else if (user.RoleId == 4)
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Student"));
                        }
                        else if (user.RoleId == 3)
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Curator"));
                        }
                        else if (user.RoleId == 2)
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Teacher"));
                        }
                        else if (user.RoleId == 1)
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin"));
                        }
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToAction("Index", "School");
                    }
                }
            }          

            return View(loginUser);
        }
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "School");
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel registrationViewModel)
        {
            User registerUser = new User();
            registerUser.Fullname = registrationViewModel.Fullname;
            registerUser.Email = registrationViewModel.Email;           
            if (ModelState.IsValid)
            {
                registerUser.Password = PasswordHasher.HashPassword(registrationViewModel.Password);
                var RoleAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                var RoleNone = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "None");
                var UserCount = await _context.Users.CountAsync();
                if (UserCount == 0)
                {
                    registerUser.Role = RoleAdmin;
                }
                else 
                {
                    registerUser.Role = RoleNone;
                }
                var existedUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == registerUser.Email && u.Password == registerUser.Password);
                if (existedUser == null)
                {
                    await _context.Users.AddAsync(registerUser);
                    await _context.SaveChangesAsync();
                }
                if(existedUser != null)
                {
                    return View();
                }
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == registerUser.Email && u.Password == registerUser.Password);
                if (user != null)
                {
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Fullname),
                        new Claim(ClaimTypes.Email, user.Email)
                    };
                    if(_context.Users.Count() == 1)
                    {
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "School");
                }
            }

            return View(registrationViewModel);
        }
        public IActionResult Account()
        {
            User user = _context.Users.SingleOrDefault(u => u.Fullname == User.Identity.Name);
            if (user != null)
            {
                return View(user);
            }
            else
                return RedirectToAction("Logout");
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "School");
        }
    }
}
