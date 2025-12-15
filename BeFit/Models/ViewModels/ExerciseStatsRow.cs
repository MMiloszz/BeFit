namespace BeFit.Models.ViewModels
{
    public class ExerciseStatsRow
    {
        public string ExerciseName { get; set; } = "";
        public int TimesPerformed { get; set; }
        public int TotalReps { get; set; }
        public decimal AvgLoad { get; set; }
        public decimal MaxLoad { get; set; }
    }
}
