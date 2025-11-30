namespace SnackToSixPack.Classes
{
    public class Profile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; } 
        public double Height { get; set; } 
        public double Waist { get; set; }
        public double Chest { get; set; }
        public double Hips { get; set; }
        public double Arm { get; set; }
        public double Thigh { get; set; }
        public FitnessLevel FitnessLevel { get; set; } 
        public Gender Gender { get; set; }
        
        public Profile(){}
    }
    public enum FitnessLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public enum Gender
    {
        Woman,
        Man,
        PreferNotToSay
    }
}
