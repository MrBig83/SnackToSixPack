namespace SnackToSixPack.Classes
{

    public class Profile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; } // in kilograms
        public double Height { get; set; } // in centimeters
        public string FitnessLevel { get; set; } //Beginner, Intermediate, Advanced
        public double Waist { get; set; }
        public double Chest { get; set; }
        public double Hips { get; set; }
        public double Arm { get; set; }
        public double Thigh { get; set; }

        public Profile(){}

        public Profile(int age, double weight, double height, string fitnessLevel, double waist, double chest, double hips, double arm, double thigh)
        {
            Age = age;
            Weight = weight;
            Height = height;
            FitnessLevel = fitnessLevel;
            Waist = waist;
            Chest = chest;
            Hips = hips;
            Arm = arm;
            Thigh = thigh;
        }
    }
}
