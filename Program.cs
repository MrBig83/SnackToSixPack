using System.Net.Mail;
using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks; // Ensure Task is available

namespace SnackToSixPack
{
    internal class Program
    {
        static async Task Main(string[] args) // Mark Main as async and return Task
        {
            //Dummy data was used for testing
            /*User user1 = new User
            {
                Id = 1,

            };
            Session.CurrentUser = user1;
            //MenuHandler.ShowMainMenu();
            Profile profile1 = new Profile
            {
                Name = "Martin",
                Age = 21,
                Weight = 67,
                Height = 112,
                FitnessLevel = "Intermediate",
                Waist = 55,
                Chest = 88,
                Hips = 54,
                Arm = 25,
                Thigh = 68
            };
            JSONHelper.SaveProfile(profile1);*/

            //AuthForms.ShowLogInForm();
            //Authentication.TwoFactorAuth();

            //Registration Does user exist 

            //RegistrationHandler.Run();
            //MenuHandler.ShowMainMenu();
            await openAIHandler.OpenAIHandler();
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
