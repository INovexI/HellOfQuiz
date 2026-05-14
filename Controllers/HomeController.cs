using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HellOfQuiz.Data;
using HellOfQuiz.Models.ViewModels;

namespace HellOfQuiz.Controllers
{
    /// <summary>
    /// Ana sayfa ve liderlik tablosunu yöneten controller.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index - Ana Sayfa
        public IActionResult Index()
        {
            var categories = _context.Categories
                .Include(c => c.Questions)
                .ToList();

            // Her kategorideki soru sayısını ViewBag ile aktar
            ViewBag.Categories = categories;
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;

            return View();
        }

    }
}
