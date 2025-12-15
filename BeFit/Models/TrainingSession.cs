using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class TrainingSession : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data i godzina rozpoczęcia jest wymagana.")]
        [Display(Name = "Start", Description = "Data i godzina rozpoczęcia sesji")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Data i godzina zakończenia jest wymagana.")]
        [Display(Name = "Koniec", Description = "Data i godzina zakończenia sesji")]
        public DateTime EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
                yield return new ValidationResult("Koniec musi być po starcie.", new[] { nameof(EndTime) });
        }
    }
}

