using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ExerciseTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExerciseTypes.OrderBy(x => x.Name).ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.ExerciseTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();

            return View(model);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExerciseType model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.ExerciseTypes.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExerciseType model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.ExerciseTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ExerciseTypes.FindAsync(id);
            if (model != null)
            {
                _context.ExerciseTypes.Remove(model);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

