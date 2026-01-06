using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollMvc.Data;
using PayrollMvc.ViewModels;
using PayrollMvc.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PayrollMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _ctx;
        public AccountController(AppDbContext ctx) => _ctx = ctx;

        // ===== Helpers =====
        private static string HashPassword(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? "")));
        }

        // ===== Login =====
        [HttpGet, AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var hash = HashPassword(model.Password);
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username && u.PasswordHash == hash);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("username", user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

            return RedirectToAction("Index", "Home");
        }

        // ===== Register (tuỳ chọn) =====
        [HttpGet, AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await _ctx.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            var user = new AppUser
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password),
                FullName = model.FullName,
                Role = "User"
            };
            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        // ===== Logout =====
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet, AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
