using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HellOfQuiz.Data;
using HellOfQuiz.Models;
using HellOfQuiz.Services;

namespace HellOfQuiz.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoomService _roomService;

        public GameController(ApplicationDbContext context, RoomService roomService)
        {
            _context = context;
            _roomService = roomService;
        }

        // GET: /Game/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Game/CreateRoom
        [HttpPost]
        public IActionResult CreateRoom(int categoryId, [FromBody] List<Question>? customQuestions)
        {
            List<Question> questionsToPlay = new();

            if (categoryId == 0) // Özel Sorular
            {
                if (customQuestions == null || customQuestions.Count == 0)
                    return BadRequest("Soru eklemediniz.");
                
                questionsToPlay = customQuestions;
            }
            else if (categoryId == -1) // Genel Kategori (Karışık)
            {
                questionsToPlay = _context.Questions
                    .OrderBy(q => Guid.NewGuid()) // Rastgele sırala
                    .Take(30) // 30 soru al
                    .ToList();
            }
            else // Belirli Kategori
            {
                questionsToPlay = _context.Questions
                    .Where(q => q.CategoryId == categoryId)
                    .OrderBy(q => Guid.NewGuid())
                    .ToList();
            }

            if (questionsToPlay.Count == 0)
                return BadRequest("Bu kategoride soru bulunamadı.");

            var pin = _roomService.CreateRoom(questionsToPlay);
            return Json(new { pin = pin, redirectUrl = $"/Game/HostLobby?pin={pin}" });
        }

        // GET: /Game/HostLobby?pin=123456
        public IActionResult HostLobby(string pin)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null) return RedirectToAction("Index", "Home");
            
            ViewBag.Pin = pin;
            return View();
        }

        // GET: /Game/HostScreen?pin=123456
        public IActionResult HostScreen(string pin)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null) return RedirectToAction("Index", "Home");
            
            ViewBag.Pin = pin;
            return View();
        }

        // GET: /Game/PlayerScreen?pin=123456&name=Ali
        public IActionResult PlayerScreen(string pin, string name)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null)
            {
                TempData["Error"] = "Oda bulunamadı!";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Geçerli bir isim girmelisiniz!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Pin = pin;
            ViewBag.PlayerName = name;
            return View();
        }

        // GET: /Game/Podium?pin=123456
        public IActionResult Podium(string pin)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null) return RedirectToAction("Index", "Home");
            
            var topPlayers = room.Players.OrderByDescending(p => p.Score).Take(3).ToList();
            return View(topPlayers);
        }
    }
}
