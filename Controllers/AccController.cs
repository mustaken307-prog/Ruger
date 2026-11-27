using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RC.Models;
using RC.Services;
using RC.ViewModels; // تأكد من استدعاء مجلد الفيو موديل
using System.Net;
using System.Security.Claims;

namespace RC.Controllers
{
    public class AccController : Controller
    {
        private readonly DbS _db;
        private readonly PwdS _pwd;

        public AccController(DbS db, PwdS pwd)
        {
            _db = db;
            _pwd = pwd;
        }

        // 1) تسجيل الدخول
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Hom");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.GetUser(model.Email);

            if (user == null || !_pwd.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Adm");

            return RedirectToAction("Index", "Sub");
        }

        // 2) إنشاء حساب
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _db.GetUser(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "هذا البريد الإلكتروني مسجل بالفعل.");
                return View(model);
            }

            try
            {
                var user = new AppUser
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = _pwd.Hash(model.Password),
                    Role = "Subscriber"
                };

                await _db.AddUser(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Sub");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // 3) تسجيل الخروج
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Hom");
        }

        // 4) رفض الوصول
        [HttpGet]
        public IActionResult Denied()
        {
            return View();
        }
    }
}