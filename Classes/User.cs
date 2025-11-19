using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnackToSixPack.Classes
{
    public class User
    {
        public int Id {get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public Profile Profile { get; set; }

        public User() { }

        public User(int id, string username, string email, string password)
        {
            Id = id;
            UserName = username;
            Email = email;
            Password = password;
        }
    }

}
