using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnackToSixPack.Classes
{
    public enum Gender { Male, Female, NonBinary, Other }
    public enum Experience { Beginner, Intermediate, Advanced }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public string Password { get; set; } // To be hashed!
        public double Weight { get; set; }
        public double Length { get; set; }
        public Gender Gender { get; set; }
        public string Adress { get; set; }
        public Experience Experience { get; set; }

        public User() { }

        public User(int id, string username, string name, string email, string phone, int age, string password, double weight, double length, Gender gender, string adress, Experience experience)
        {
            Id = id;
            UserName = username;
            Name = name;
            Email = email;
            Phone = phone;
            Age = age;
            Password = password;
            Weight = weight;
            Length = length;
            Gender = gender;
            Adress = adress;
            Experience = experience;
        }
    }
}
