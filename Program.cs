using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using System.Security.Cryptography.X509Certificates;

namespace SnackToSixPack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //MenuHandler.ShowMainMenu();
            //AuthForms.ShowLogInForm();
            //Authentication.TwoFactorAuth();

            //AuthForms.ShowLogInForm();

            //Registration Does user exist 

            FeatureRegisterForm.Run();
      
            //var from = new FeatureRegisterForm();

            //// ======= Detta är bara för att provköra hämtning av JSON och User-klassen =======
            //List<User> users = JsonHelper.LoadUsers();
            //foreach (var user in users)
            //{
            //    Console.WriteLine(user.Name);
            //}
            //// ======= Slut på provkörning =======
            ///


        }
    }
}
