using System;
using System.Collections.Generic;

public class WorkoutPlan
{
    public string WorkoutPlanId { get; set; }
    public string UserId { get; set; }
    public string PlanName { get; set; }
    public string Goal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Notes { get; set; }
    public List<Workout> Workouts { get; set; } = new List<Workout>();
}
