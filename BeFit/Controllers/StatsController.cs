using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models.ViewModels;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StatsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User)!;
            var from = DateTime.Now.AddDays(-28);

            var data = await _context.PerformedExercises
                .Where(pe => pe.UserId == uid)
                .Where(pe => _context.TrainingSessions.Any(ts =>
                    ts.Id == pe.TrainingSessionId &&
                    ts.UserId == uid &&
                    ts.StartTime >= from))
                .Include(pe => pe.ExerciseType)
                .ToListAsync();

            var rows = data
                .GroupBy(x => x.ExerciseType!.Name)
                .Select(g => new ExerciseStatsRow
                {
                    ExerciseName = g.Key,
                    TimesPerformed = g.Count(),
                    TotalReps = g.Sum(x => x.Sets * x.RepsPerSet),
                    AvgLoad = g.Average(x => x.LoadKg),
                    MaxLoad = g.Max(x => x.LoadKg)
                })
                .OrderBy(r => r.ExerciseName)
                .ToList();

            return View(rows);
        }
    }
}
