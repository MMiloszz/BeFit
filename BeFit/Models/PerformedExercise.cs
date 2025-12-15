using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class PerformedExercise
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }

        [Required]
        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [Range(0, 1000, ErrorMessage = "Obciążenie musi być w zakresie 0–1000.")]
        [Display(Name = "Obciążenie (kg)", Description = "Dla masy ciała wpisz 0")]
        public decimal LoadKg { get; set; }

        [Range(1, 50, ErrorMessage = "Liczba serii musi być w zakresie 1–50.")]
        [Display(Name = "Liczba serii")]
        public int Sets { get; set; }

        [Range(1, 500, ErrorMessage = "Liczba powtórzeń musi być w zakresie 1–500.")]
        [Display(Name = "Powtórzeń w serii")]
        public int RepsPerSet { get; set; }

        public TrainingSession? TrainingSession { get; set; }
        public ExerciseType? ExerciseType { get; set; }
    }
}
