using System;
using System.Collections.Generic;

public class Workout
{
    public string WorkoutId { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public List<Exercise> Exercises { get; set; } = new List<Exercise>();
}
