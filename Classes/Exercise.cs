public class Exercise
{
    public string ExerciseId { get; set; }
    public string Name { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public double? Weight { get; set; }   // Använd nullable för att tillåta null i JSON
    public int? RestTime { get; set; }    // Sekunder, nullable
    public string Notes { get; set; }
    public bool IsCompleted { get; set; }
}
