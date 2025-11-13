using System.Net.Mail;
using SnackToSixPack.Classes;
using SnackToSixPack.Handlers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks; // Ensure Task is available

namespace SnackToSixPack
{
    internal class Program
    {
        static async Task Main(string[] args) 
        {
            if (Console.IsInputRedirected)
            {
                Console.WriteLine("CI mode – hoppar över interaktiv del.");
                return;
            }
            await MenuHandler.ShowMainMenu();



        }
    }
}
