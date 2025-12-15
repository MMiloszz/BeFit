using BeFit.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExerciseType> ExerciseTypes => Set<ExerciseType>();
        public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
        public DbSet<PerformedExercise> PerformedExercises => Set<PerformedExercise>();
    }
}
