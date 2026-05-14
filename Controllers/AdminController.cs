using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HellOfQuiz.Data;
using HellOfQuiz.Models;

namespace HellOfQuiz.Controllers
{
    /// <summary>
    /// Sadece admin kullanıcılara açık olan yönetim paneli controller'ı.
    /// Soru ve kategori CRUD işlemleri burada yapılır.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin kontrolü - Admin değilse ana sayfaya yönlendir
        private bool IsAdmin()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            return isAdmin == "True";
        }

        // GET: /Admin/Index - Admin Paneli Ana Sayfası
        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz bulunmuyor!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalQuestions = _context.Questions.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalGames = 0; // Skorlar artık hafızada

            return View();
        }

        // ============ SORU YÖNETİMİ ============

        // GET: /Admin/Questions - Tüm soruları listele
        public IActionResult Questions(int? categoryId)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var query = _context.Questions.Include(q => q.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(q => q.CategoryId == categoryId.Value);

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            return View(query.OrderBy(q => q.CategoryId).ThenBy(q => q.Id).ToList());
        }

        // GET: /Admin/CreateQuestion - Yeni soru ekleme formu
        public IActionResult CreateQuestion()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Admin/CreateQuestion - Yeni soru kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateQuestion(Question question)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Questions.Add(question);
                _context.SaveChanges();
                TempData["Success"] = "Soru başarıyla eklendi! ✅";
                return RedirectToAction("Questions");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(question);
        }

        // GET: /Admin/EditQuestion/{id} - Soru düzenleme formu
        public IActionResult EditQuestion(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var question = _context.Questions.Find(id);
            if (question == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(question);
        }

        // POST: /Admin/EditQuestion/{id} - Soruyu güncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditQuestion(int id, Question question)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            if (id != question.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Questions.Update(question);
                _context.SaveChanges();
                TempData["Success"] = "Soru başarıyla güncellendi! ✅";
                return RedirectToAction("Questions");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(question);
        }

        // POST: /Admin/DeleteQuestion/{id} - Soruyu sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestion(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var question = _context.Questions.Find(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                _context.SaveChanges();
                TempData["Success"] = "Soru silindi! 🗑️";
            }

            return RedirectToAction("Questions");
        }

        // ============ KATEGORİ YÖNETİMİ ============

        // GET: /Admin/Categories - Tüm kategorileri listele
        public IActionResult Categories()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var categories = _context.Categories.Include(c => c.Questions).ToList();
            return View(categories);
        }

        // GET: /Admin/CreateCategory - Yeni kategori formu
        public IActionResult CreateCategory()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Admin/CreateCategory - Yeni kategoriyi kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["Success"] = "Kategori başarıyla eklendi! ✅";
                return RedirectToAction("Categories");
            }

            return View(category);
        }

        // POST: /Admin/DeleteCategory/{id} - Kategoriyi sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["Success"] = "Kategori silindi! 🗑️";
            }

            return RedirectToAction("Categories");
        }

        // ============ KULLANICI YÖNETİMİ ============

        // GET: /Admin/Users - Tüm kullanıcıları listele
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var users = _context.Users.ToList();
            return View(users);
        }
    }
}
