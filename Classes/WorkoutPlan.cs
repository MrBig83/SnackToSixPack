using System;
using System.Collections.Generic;

public class WorkoutPlan
{
    public string PlanName { get; set; }
    public string Goal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Workout> Workouts { get; set; } = new List<Workout>();
}
