using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TrainingSessionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string CurrentUserId() => _userManager.GetUserId(User)!;

        public async Task<IActionResult> Index()
        {
            var uid = CurrentUserId();
            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == uid)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var uid = CurrentUserId();
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == uid);

            if (session == null) return NotFound();
            return View(session);
        }

        public IActionResult Create()
        {
            return View(new TrainingSession
            {
                StartTime = System.DateTime.Now,
                EndTime = System.DateTime.Now.AddHours(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingSession session)
        {
            session.UserId = CurrentUserId(); // automatycznie, bez pola w formularzu

            if (!ModelState.IsValid) return View(session);

            _context.Add(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var uid = CurrentUserId();
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == uid);

            if (session == null) return NotFound();
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingSession posted)
        {
            if (id != posted.Id) return NotFound();

            var uid = CurrentUserId();
            var existing = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == uid);

            if (existing == null) return NotFound();

            existing.StartTime = posted.StartTime;
            existing.EndTime = posted.EndTime;

            if (!TryValidateModel(existing)) return View(existing);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var uid = CurrentUserId();
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == uid);

            if (session == null) return NotFound();
            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = CurrentUserId();
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == uid);

            if (session == null) return NotFound();

            _context.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
