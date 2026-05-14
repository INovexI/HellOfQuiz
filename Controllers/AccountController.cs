using Microsoft.AspNetCore.Mvc;
using HellOfQuiz.Data;
using HellOfQuiz.Models;
using HellOfQuiz.Models.ViewModels;

namespace HellOfQuiz.Controllers
{
    /// <summary>
    /// Kullanıcı kayıt, giriş ve çıkış işlemlerini yöneten controller.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Zaten giriş yapmışsa ana sayfaya yönlendir
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kullanıcıyı email ile veritabanında ara
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            // Kullanıcı bulunamazsa veya şifre yanlışsa hata göster
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı!");
                return View(model);
            }

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

            TempData["Success"] = $"Hoş geldin, {user.Username}! 🎉";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Email veya kullanıcı adı zaten kullanılıyorsa hata ver
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            // Yeni kullanıcıyı oluştur ve şifreyi hashle
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Kaydettikten sonra otomatik giriş yaptır
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("IsAdmin", "False");

            TempData["Success"] = "Hesabın başarıyla oluşturuldu! Hadi oynamaya başla! 🚀";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Güvenli çıkış yapıldı. Görüşürüz! 👋";
            return RedirectToAction("Login");
        }
    }
}
