using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;

namespace SnackToSixPack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //MenuHandler.ShowUserMenu();
            //AuthForms.ShowLogInForm();
            Authentication.TwoFactorAuth(); 
            
            //AuthForms.ShowLogInForm();

            //// ======= Detta är bara för att provköra hämtning av JSON och User-klassen =======
            //List<User> users = JsonHelper.LoadUsers();
            //foreach (var user in users)
            //{
            //    Console.WriteLine(user.Name);
            //}
            //// ======= Slut på provkörning =======
        }
    }
}
