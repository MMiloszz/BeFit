using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Nazwa musi mieć 2–60 znaków.")]
        [Display(Name = "Nazwa ćwiczenia", Description = "Np. Przysiad, Wyciskanie leżąc")]
        public string Name { get; set; } = string.Empty;
    }
}
