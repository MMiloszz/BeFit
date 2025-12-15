using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class PerformedExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PerformedExercisesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string CurrentUserId() => _userManager.GetUserId(User)!;

        private async Task FillDropdowns(string uid, int? sessionId = null, int? typeId = null)
        {
            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == uid)
                .OrderByDescending(s => s.StartTime)
                .Select(s => new
                {
                    s.Id,
                    Label = s.StartTime.ToString("yyyy-MM-dd HH:mm") + " – " + s.EndTime.ToString("HH:mm")
                })
                .ToListAsync();

            var types = await _context.ExerciseTypes
                .OrderBy(t => t.Name)
                .ToListAsync();

            ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Label", sessionId);
            ViewData["ExerciseTypeId"] = new SelectList(types, "Id", "Name", typeId);
        }

        public async Task<IActionResult> Index()
        {
            var uid = CurrentUserId();
            var list = await _context.PerformedExercises
                .Where(e => e.UserId == uid)
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var uid = CurrentUserId();
            var model = await _context.PerformedExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == uid);

            if (model == null) return NotFound();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var uid = CurrentUserId();
            await FillDropdowns(uid);
            return View(new PerformedExercise { Sets = 3, RepsPerSet = 10, LoadKg = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerformedExercise model)
        {
            var uid = CurrentUserId();
            model.UserId = uid; // automatycznie

            // zabezpieczenie: nie da się przypisać ćwiczenia do cudzej sesji
            var sessionOk = await _context.TrainingSessions
                .AnyAsync(s => s.Id == model.TrainingSessionId && s.UserId == uid);

            if (!sessionOk)
                ModelState.AddModelError("TrainingSessionId", "Wybrana sesja nie należy do Ciebie.");

            if (!ModelState.IsValid)
            {
                await FillDropdowns(uid, model.TrainingSessionId, model.ExerciseTypeId);
                return View(model);
            }

            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var uid = CurrentUserId();
            var model = await _context.PerformedExercises
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == uid);

            if (model == null) return NotFound();

            await FillDropdowns(uid, model.TrainingSessionId, model.ExerciseTypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PerformedExercise posted)
        {
            if (id != posted.Id) return NotFound();

            var uid = CurrentUserId();
            var existing = await _context.PerformedExercises
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == uid);

            if (existing == null) return NotFound();

            var sessionOk = await _context.TrainingSessions
                .AnyAsync(s => s.Id == posted.TrainingSessionId && s.UserId == uid);

            if (!sessionOk)
                ModelState.AddModelError("TrainingSessionId", "Wybrana sesja nie należy do Ciebie.");

            existing.TrainingSessionId = posted.TrainingSessionId;
            existing.ExerciseTypeId = posted.ExerciseTypeId;
            existing.LoadKg = posted.LoadKg;
            existing.Sets = posted.Sets;
            existing.RepsPerSet = posted.RepsPerSet;

            if (!TryValidateModel(existing))
            {
                await FillDropdowns(uid, existing.TrainingSessionId, existing.ExerciseTypeId);
                return View(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var uid = CurrentUserId();
            var model = await _context.PerformedExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == uid);

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = CurrentUserId();
            var model = await _context.PerformedExercises
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == uid);

            if (model == null) return NotFound();

            _context.PerformedExercises.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
